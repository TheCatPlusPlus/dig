#pragma once

#include <sdkddkver.h>
#include <windows.h>
#include <objbase.h>
#include <comdef.h>
#include <atlbase.h>

#include <boost/preprocessor.hpp>
#include <fmt/format.h>

#include <utility>
#include <exception>
#include <string>
#include <mutex>
#include <cstdint>

#define DIG_ABI extern "C" __declspec(dllexport)
#define DIG_ABI_IMPL_NAME(name) BOOST_PP_CAT(do_, name)
#define DIG_ABI_IMPL_PROTECT(name, ...) \
	return ::dig::protect(BOOST_PP_WSTRINGIZE(name), DIG_ABI_IMPL_NAME(name), __VA_ARGS__)
#define DIG_ABI_RESULT const wchar_t*

#define DIG_ABI_CALL0(name) \
	void DIG_ABI_IMPL_NAME(name)(); \
	DIG_ABI DIG_ABI_RESULT name() { DIG_ABI_IMPL_PROTECT(name); } \
	void DIG_ABI_IMPL_NAME(name)()

#define DIG_ABI_CALL1(name, T0, a0) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0) { DIG_ABI_IMPL_PROTECT(name, a0); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0)

#define DIG_ABI_CALL2(name, T0, a0, T1, a1) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1) { DIG_ABI_IMPL_PROTECT(name, a0, a1); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1)

#define DIG_ABI_CALL3(name, T0, a0, T1, a1, T2, a2) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2)

#define DIG_ABI_CALL4(name, T0, a0, T1, a1, T2, a2, T3, a3) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2, T3 a3) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2, a3); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3)

#define DIG_ABI_CALL5(name, T0, a0, T1, a1, T2, a2, T3, a3, T4, a4) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2, a3, a4); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)

#define DIG_ABI_CALL6(name, T0, a0, T1, a1, T2, a2, T3, a3, T4, a4, T5, a5) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2, a3, a4, a5); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)

#define DIG_ABI_CALL7(name, T0, a0, T1, a1, T2, a2, T3, a3, T4, a4, T5, a5, T6, a6) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2, a3, a4, a5, a6); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)

#define DIG_ABI_CALL8(name, T0, a0, T1, a1, T2, a2, T3, a3, T4, a4, T5, a5, T6, a6, T7, a7) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2, a3, a4, a5, a6, a7); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)

#define DIG_ABI_CALL9(name, T0, a0, T1, a1, T2, a2, T3, a3, T4, a4, T5, a5, T6, a6, T7, a7, T8, a8) \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8); \
	DIG_ABI DIG_ABI_RESULT name(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8) { DIG_ABI_IMPL_PROTECT(name, a0, a1, a2, a3, a4, a5, a6, a7, a8); } \
	void DIG_ABI_IMPL_NAME(name)(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)

#define DIG_FAIL(format, ...) \
	::dig::fail(__FUNCTIONW__ L": " format, __VA_ARGS__)

#define DIG_FAIL_WINAPI(format, ...) \
	::dig::fail_winapi(__FUNCTIONW__ L": " format, __VA_ARGS__)

#define DIG_FAIL_WINAPI_CODE(code, format, ...) \
	::dig::fail_winapi(code, __FUNCTIONW__ L": " format, __VA_ARGS__)

namespace dig
{
	using String = const wchar_t*;
	using LogFunc = void (*)(int32_t level, String message);

	extern LogFunc g_log_func;
	extern std::mutex g_log_mutex;
	extern thread_local std::wstring g_last_error;

	struct FatalError : virtual std::exception
	{
		explicit FatalError(const std::wstring& message)
			: m_message(message)
		{
		}

		char const* what() const override
		{
			return "FatalError uses wchar_t error message (use what_w() instead)";
		}

		String what_w() const
		{
			return m_message.c_str();
		}

	private:
		std::wstring m_message;
	};

	namespace detail
	{
		template <typename...Args>
		void log(int32_t level, String format, Args&&... args)
		{
			if (!g_log_func)
			{
				return;
			}

			std::lock_guard<std::mutex> lock(g_log_mutex);
			auto message = fmt::format(format, std::forward<Args>(args)...);
			g_log_func(level, message.c_str());
		}

		inline std::wstring format_winapi(DWORD error_code)
		{
			wchar_t* raw_ptr;

			auto flags = FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_IGNORE_INSERTS;
			auto size = FormatMessageW(
				flags,
				nullptr,
				error_code,
				MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
				reinterpret_cast<wchar_t*>(&raw_ptr),
				0,
				nullptr);

			std::wstring error_message;
			if (size == 0)
			{
				error_message = fmt::format(
					L"error code 0x{:X} (FormatMessage failed with error code 0x{:X})",
					error_code, GetLastError()
				);
			}
			else
			{
				std::unique_ptr<wchar_t, decltype(&LocalFree)> ptr(raw_ptr, LocalFree);
				error_message = std::wstring(raw_ptr, raw_ptr + size - 2);
			}

			return error_message;
		}

		template <typename... Args>
		std::wstring format_winapi(DWORD error_code, String format, Args&&... args)
		{
			auto error_message = format_winapi(error_code);
			auto message = fmt::format(format, std::forward<Args>(args)...);
			auto full_message = fmt::format(L"{}: {}", message, error_message);
			return full_message;
		}

		template <typename... Args>
		std::wstring format_winapi(String format, Args&&... args)
		{
			return format_winapi(GetLastError(), format, std::forward<Args>(args)...);
		}

		template <typename... Args>
		std::wstring format_hresult(HRESULT result, String format, Args&&... args)
		{
			_com_error error{result};
			auto message = fmt::format(format, std::forward<Args>(args)...);
			auto full_message = fmt::format(L"{}: {}", message, error.ErrorMessage());
			return full_message;
		}

		template <typename... Args>
		String set_error(String format, Args&&... args)
		{
			g_last_error = fmt::format(format, std::forward<Args>(args)...);
			return g_last_error.c_str();
		}
	}

	template <typename...Args>
	void log_debug(String format, Args&&... args)
	{
		detail::log(1, format, std::forward<Args>(args)...);
	}

	template <typename...Args>
	void log_info(String format, Args&&... args)
	{
		detail::log(2, format, std::forward<Args>(args)...);
	}

	template <typename...Args>
	void log_warning(String format, Args&&... args)
	{
		detail::log(3, format, std::forward<Args>(args)...);
	}

	template <typename...Args>
	void log_error(String format, Args&&... args)
	{
		detail::log(4, format, std::forward<Args>(args)...);
	}

	template <typename...Args>
	[[noreturn]]
	void fail(const std::wstring& format, Args&&... args)
	{
		auto message = fmt::format(format, std::forward<Args>(args)...);
		throw FatalError{message};
	}

	template <typename... Args>
	[[noreturn]]
	void fail_winapi(DWORD error_code, String format, Args&&... args)
	{
		auto full_message = detail::format_winapi(error_code, format, std::forward<Args>(args)...);
		fail(full_message);
	}

	template <typename... Args>
	[[noreturn]]
	void fail_winapi(String format, Args&&... args)
	{
		fail_winapi(GetLastError(), format, std::forward<Args>(args)...);
	}

	template <typename... Args>
	[[noreturn]]
	void fail_hresult(HRESULT result, String format, Args&&... args)
	{
		auto message = detail::format_hresult(result, format, std::forward<Args>(args)...);
		fail(message);
	}

	template <typename... Args>
	void check_hresult(HRESULT result, String format, Args&&... args)
	{
		if (FAILED(result))
		{
			fail_hresult(result, format, std::forward<Args>(args)...);
		}
	}

	inline std::wstring to_utf16(const std::string& s)
	{
		if (s.empty())
		{
			return L"";
		}

		std::wstring buffer{};

		auto size = MultiByteToWideChar(CP_UTF8, 0, &s[0], static_cast<int>(s.size()), nullptr, 0);
		if (size == 0)
		{
			DIG_FAIL_WINAPI(L"failed to get the buffer size");
		}

		buffer.resize(size);

		size = MultiByteToWideChar(CP_UTF8, 0, &s[0], static_cast<int>(s.size()), &buffer[0], size);
		if (size == 0)
		{
			DIG_FAIL_WINAPI(L"failed to transcode the string");
		}

		return buffer;
	}

	inline std::string to_utf8(const std::wstring& s)
	{
		if (s.empty())
		{
			return "";
		}

		std::string buffer{};

		auto size = WideCharToMultiByte(CP_UTF8, 0, &s[0], static_cast<int>(s.size()), nullptr, 0, nullptr, nullptr);
		if (size == 0)
		{
			DIG_FAIL_WINAPI(L"failed to get the buffer size");
		}

		buffer.resize(size);

		size = WideCharToMultiByte(CP_UTF8, 0, &s[0], static_cast<int>(s.size()), &buffer[0], size, nullptr, nullptr);
		if (size == 0)
		{
			DIG_FAIL_WINAPI(L"failed to transcode the string");
		}

		return buffer;
	}

	template <typename Fn, typename... Args>
	String protect(const wchar_t* label, Fn&& fn, Args&&... args)
	{
		try
		{
			fn(std::forward<Args>(args)...);
			return nullptr;
		}
		catch (const FatalError& e)
		{
			return detail::set_error(L"{} failed: {}", label, e.what_w());
		}
		catch (const std::exception& e)
		{
			auto&& type = typeid(e);
			std::wstring type_name;
			std::wstring message;

			try
			{
				type_name = to_utf16(type.name());
			}
			catch (const FatalError& te)
			{
				type_name = fmt::format(L"<type name transcode failed: {}>", te.what_w());
			}

			try
			{
				message = to_utf16(e.what());
			}
			catch (const FatalError& te)
			{
				message = fmt::format(L"<message transcode failed: {}>", te.what_w());
			}

			return detail::set_error(L"{} failed: {}: {}", label, type_name, message);
		}
	}
}
