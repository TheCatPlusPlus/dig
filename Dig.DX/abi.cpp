#include <abi.hpp>

#include <d3d11.h>
#include <DirectXMath.h>
#include <DirectXMesh.h>
#include <DirectXTK/DDSTextureLoader.h>

// ReSharper disable CppInconsistentNaming

#define DIG_ABI_CHECK_NULLPTR(var) \
	do { if ((var) == nullptr) { DIG_FAIL(BOOST_PP_WSTRINGIZE(var) L" cannot be nullptr"); } } while (false)
#define DIG_ABI_REQUIRE_NULLPTR(var) \
	do { if ((var) != nullptr) { DIG_FAIL(BOOST_PP_WSTRINGIZE(var) L" must be nullptr"); } } while (false)

namespace dig
{
	LogFunc g_log_func{};
	std::mutex g_log_mutex{};
	thread_local std::wstring g_last_error{};

	struct TextureParams
	{
		ID3D11Device* Device;
		const uint8_t* Buffer;
		uint64_t BufferSize;
		D3D11_USAGE Usage;
		uint32_t BindFlags;
		uint32_t CPUAccessFlags;
		uint32_t MiscFlags;
		bool ForceSRGB;
		uint64_t MaxSize;

		TextureParams(const TextureParams& other) = delete;
		TextureParams(TextureParams&& other) noexcept = delete;
		TextureParams& operator=(const TextureParams& other) = delete;
		TextureParams& operator=(TextureParams&& other) noexcept = delete;
	};

	struct MeshBuilder
	{
		std::vector<uint16_t> Triangles{};
		std::vector<DirectX::XMFLOAT3> Vertices{};
		std::vector<DirectX::XMFLOAT2> UVs{};
		std::vector<DirectX::XMFLOAT3> Normals{};
		std::vector<DirectX::XMFLOAT3> Tangents{};
		std::vector<DirectX::XMFLOAT3> Bitangents{};

		MeshBuilder() = default;
		MeshBuilder(const MeshBuilder& other) = delete;
		MeshBuilder(MeshBuilder&& other) noexcept = delete;
		MeshBuilder& operator=(const MeshBuilder& other) = delete;
		MeshBuilder& operator=(MeshBuilder&& other) noexcept = delete;

		uint16_t add_vertex(DirectX::XMFLOAT3 position, DirectX::XMFLOAT2 uv)
		{
			auto index = static_cast<uint16_t>(Vertices.size());
			Vertices.emplace_back(position);
			UVs.emplace_back(uv);
			Normals.emplace_back();
			Tangents.emplace_back();
			Bitangents.emplace_back();
			return index;
		}

		void add_triangle(uint16_t a, uint16_t b, uint16_t c)
		{
			Triangles.emplace_back(a);
			Triangles.emplace_back(b);
			Triangles.emplace_back(c);
		}

		void add_quad(
			DirectX::XMFLOAT3 position0, DirectX::XMFLOAT2 uv0,
			DirectX::XMFLOAT3 position1, DirectX::XMFLOAT2 uv1,
			DirectX::XMFLOAT3 position2, DirectX::XMFLOAT2 uv2,
			DirectX::XMFLOAT3 position3, DirectX::XMFLOAT2 uv3
		)
		{
			auto idx0 = add_vertex(position0, uv0);
			auto idx1 = add_vertex(position1, uv1);
			auto idx2 = add_vertex(position2, uv2);
			auto idx3 = add_vertex(position3, uv3);
			add_triangle(idx0, idx1, idx2);
			add_triangle(idx0, idx2, idx3);
		}

		void finish()
		{
			auto result = DirectX::ComputeNormals(
				Triangles.data(), Triangles.size(),
				Vertices.data(), Vertices.size(),
				DirectX::CNORM_DEFAULT, Normals.data()
			);
			check_hresult(result, L"ComputeNormals");

			result = DirectX::ComputeTangentFrame(
				Triangles.data(), Triangles.size(),
				Vertices.data(), Normals.data(), UVs.data(), Vertices.size(),
				Tangents.data(), Bitangents.data()
			);
			check_hresult(result, L"ComputeTangentFrame");
		}
	};

	struct MeshData
	{
		uint64_t VertexCount;
		uint64_t TriangleCount;
		uint16_t* Triangles;
		DirectX::XMFLOAT3* Vertices;
		DirectX::XMFLOAT2* UVs;
		DirectX::XMFLOAT3* Normals;
		DirectX::XMFLOAT3* Tangents;
		DirectX::XMFLOAT3* Bitangents;

		explicit MeshData(MeshBuilder& builder)
			: VertexCount(builder.Vertices.size())
			, TriangleCount(builder.Triangles.size())
			, Triangles(new uint16_t[TriangleCount]{})
			, Vertices(new DirectX::XMFLOAT3[VertexCount]{})
			, UVs(new DirectX::XMFLOAT2[VertexCount]{})
			, Normals(new DirectX::XMFLOAT3[VertexCount]{})
			, Tangents(new DirectX::XMFLOAT3[VertexCount]{})
			, Bitangents(new DirectX::XMFLOAT3[VertexCount]{})
		{
			std::copy(builder.Triangles.begin(), builder.Triangles.end(), Triangles);
			std::copy(builder.Vertices.begin(), builder.Vertices.end(), Vertices);
			std::copy(builder.UVs.begin(), builder.UVs.end(), UVs);
			std::copy(builder.Normals.begin(), builder.Normals.end(), Normals);
			std::copy(builder.Tangents.begin(), builder.Tangents.end(), Tangents);
			std::copy(builder.Bitangents.begin(), builder.Bitangents.end(), Bitangents);
		}

		~MeshData()
		{
			delete Triangles;
			delete Vertices;
			delete UVs;
			delete Normals;
			delete Tangents;
			delete Bitangents;
		}

		MeshData(const MeshData& other) = delete;
		MeshData(MeshData&& other) noexcept = delete;
		MeshData& operator=(const MeshData& other) = delete;
		MeshData& operator=(MeshData&& other) noexcept = delete;
	};

	DIG_ABI_CALL1(Init, LogFunc, log)
	{
		{
			std::lock_guard<std::mutex> lock(g_log_mutex);
			g_log_func = log;
		}

		log_info(L"Dig.DX ready");
	}

	DIG_ABI_CALL4(TextureCreateFromDDS,
		TextureParams*, params,
		ID3D11Resource**, texture,
		ID3D11ShaderResourceView**, view,
		DirectX::DDS_ALPHA_MODE*, alpha_mode
	)
	{
		DIG_ABI_CHECK_NULLPTR(params);
		DIG_ABI_CHECK_NULLPTR(texture);
		DIG_ABI_CHECK_NULLPTR(view);
		DIG_ABI_CHECK_NULLPTR(alpha_mode);
		DIG_ABI_REQUIRE_NULLPTR(*texture);
		DIG_ABI_REQUIRE_NULLPTR(*view);

		auto result = DirectX::CreateDDSTextureFromMemoryEx(
			params->Device, params->Buffer, params->BufferSize,
			params->MaxSize, params->Usage, params->BindFlags,
			params->CPUAccessFlags, params->MiscFlags, params->ForceSRGB,
			texture, view, alpha_mode
		);
		check_hresult(result, L"CreateDDSTextureFromMemoryEx");
	}

	DIG_ABI_CALL1(MeshCreateCube, MeshData**, mesh)
	{
		DIG_ABI_CHECK_NULLPTR(mesh);
		DIG_ABI_REQUIRE_NULLPTR(*mesh);

		MeshBuilder builder{};

		DirectX::XMFLOAT2 uv_top_left(0.0f, 0.0f);
		DirectX::XMFLOAT2 uv_top_right(1.0f, 0.0f);
		DirectX::XMFLOAT2 uv_bottom_right(1.0f, 1.0f);
		DirectX::XMFLOAT2 uv_bottom_left(0.0f, 1.0f);

		// top
		builder.add_quad
		(
			{-1.0f, 1.0f, -1.0f}, uv_bottom_left,
			{-1.0f, 1.0f, 1.0f}, uv_top_left,
			{1.0f, 1.0f, 1.0f}, uv_top_right,
			{1.0f, 1.0f, -1.0f}, uv_bottom_right
		);

		// bottom
		builder.add_quad
		(
			{-1.0f, -1.0f, -1.0f}, uv_bottom_right,
			{1.0f, -1.0f, -1.0f}, uv_bottom_left,
			{1.0f, -1.0f, 1.0f}, uv_top_left,
			{-1.0f, -1.0f, 1.0f}, uv_top_right
		);

		// front
		builder.add_quad
		(
			{-1.0f, -1.0f, -1.0f}, uv_bottom_left,
			{-1.0f, 1.0f, -1.0f}, uv_top_left,
			{1.0f, 1.0f, -1.0f}, uv_top_right,
			{1.0f, -1.0f, -1.0f}, uv_bottom_right
		);

		// back
		builder.add_quad
		(
			{-1.0f, -1.0f, 1.0f}, uv_bottom_right,
			{1.0f, -1.0f, 1.0f}, uv_bottom_left,
			{1.0f, 1.0f, 1.0f}, uv_top_left,
			{-1.0f, 1.0f, 1.0f}, uv_top_right
		);

		// left
		builder.add_quad
		(
			{-1.0f, -1.0f, 1.0f}, uv_bottom_left,
			{-1.0f, 1.0f, 1.0f}, uv_top_left,
			{-1.0f, 1.0f, -1.0f}, uv_top_right,
			{-1.0f, -1.0f, -1.0f}, uv_bottom_right
		);

		// right
		builder.add_quad
		(
			{1.0f, -1.0f, -1.0f}, uv_bottom_left,
			{1.0f, 1.0f, -1.0f}, uv_top_left,
			{1.0f, 1.0f, 1.0f}, uv_top_right,
			{1.0f, -1.0f, 1.0f}, uv_bottom_right
		);

		builder.finish();
		*mesh = new MeshData{builder};
	}

	DIG_ABI_CALL1(MeshDestroy, MeshData*, mesh)
	{
		DIG_ABI_CHECK_NULLPTR(mesh);
		delete mesh;
	}
}
