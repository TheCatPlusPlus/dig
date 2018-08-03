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
}
