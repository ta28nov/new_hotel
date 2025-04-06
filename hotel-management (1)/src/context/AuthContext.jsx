"\"use client"

// Import các thư viện cần thiết
import { createContext, useState, useEffect, useContext } from "react"
import { useNavigate } from "react-router-dom"
import { toast } from "react-toastify"
import authService from "../services/authService"
import { TOKEN_KEY, USER_KEY } from "../config/constants"

// Tạo context xác thực
const AuthContext = createContext()

// Hook tùy chỉnh để sử dụng context xác thực
export const useAuth = () => useContext(AuthContext)

export const AuthProvider = ({ children }) => {
  // Các state quản lý thông tin người dùng và trạng thái
  const [currentUser, setCurrentUser] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const navigate = useNavigate()

  // Kiểm tra token khi component được tải
  useEffect(() => {
    const checkAuth = async () => {
      const token = localStorage.getItem(TOKEN_KEY)
      const savedUser = localStorage.getItem(USER_KEY)

      if (token) {
        try {
          if (savedUser) {
            // Sử dụng thông tin người dùng đã lưu trước khi gọi API
            setCurrentUser(JSON.parse(savedUser))
          }

          // Gọi API để lấy thông tin người dùng hiện tại
          const response = await authService.getCurrentUser()
          const userData = response.data

          // Cập nhật thông tin người dùng từ API
          setCurrentUser(userData)
          localStorage.setItem(USER_KEY, JSON.stringify(userData))
        } catch (error) {
          console.error("Error fetching current user:", error)
          // Xóa token nếu không hợp lệ
          localStorage.removeItem(TOKEN_KEY)
          localStorage.removeItem(USER_KEY)
          setCurrentUser(null)
        }
      }
      setLoading(false)
    }

    checkAuth()
  }, [])

  /**
   * Đăng nhập người dùng
   * @param {string} email - Email đăng nhập
   * @param {string} password - Mật khẩu
   * @returns {Object} - Thông tin người dùng
   */
  const login = async (email, password) => {
    try {
      setLoading(true)
      setError(null)
      const response = await authService.login(email, password)
      const { token, user } = response.data

      // Lưu token và thông tin người dùng vào localStorage
      localStorage.setItem(TOKEN_KEY, token)
      localStorage.setItem(USER_KEY, JSON.stringify(user))
      setCurrentUser(user)

      toast.success("Đăng nhập thành công!")

      // Chuyển hướng dựa trên vai trò
      if (user.role === "admin" || user.role === "employee") {
        navigate("/admin/dashboard")
      } else {
        navigate("/")
      }

      return user
    } catch (error) {
      const errorMessage = error.response?.data?.message || "Đăng nhập thất bại. Vui lòng kiểm tra thông tin đăng nhập."
      setError(errorMessage)
      toast.error(errorMessage)
      throw error
    } finally {
      setLoading(false)
    }
  }

  /**
   * Đăng ký người dùng mới
   * @param {Object} userData - Thông tin người dùng
   * @returns {Object} - Kết quả đăng ký
   */
  const register = async (userData) => {
    try {
      setLoading(true)
      setError(null)
      const response = await authService.register(userData)
      toast.success("Đăng ký thành công! Vui lòng đăng nhập.")
      navigate("/login")
      return response.data
    } catch (error) {
      const errorMessage = error.response?.data?.message || "Đăng ký thất bại. Vui lòng thử lại."
      setError(errorMessage)
      toast.error(errorMessage)
      throw error
    } finally {
      setLoading(false)
    }
  }

  /**
   * Đăng xuất người dùng
   */
  const logout = async () => {
    try {
      // Gọi API đăng xuất nếu backend yêu cầu
      if (currentUser) {
        await authService.logout()
      }
    } catch (error) {
      console.error("Logout error:", error)
    } finally {
      // Luôn xóa dữ liệu local ngay cả khi API thất bại
      localStorage.removeItem(TOKEN_KEY)
      localStorage.removeItem(USER_KEY)
      setCurrentUser(null)
      toast.info("Đã đăng xuất")
      navigate("/")
    }
  }

  /**
   * Cập nhật thông tin người dùng
   * @param {Object} userData - Thông tin người dùng cần cập nhật
   * @returns {Object} - Thông tin người dùng đã cập nhật
   */
  const updateProfile = async (userData) => {
    try {
      setLoading(true)
      setError(null)
      const response = await authService.updateProfile(userData)
      const updatedUser = response.data

      // Cập nhật thông tin người dùng trong state và localStorage
      setCurrentUser({ ...currentUser, ...updatedUser })
      localStorage.setItem(USER_KEY, JSON.stringify({ ...currentUser, ...updatedUser }))

      toast.success("Cập nhật thông tin thành công!")
      return updatedUser
    } catch (error) {
      const errorMessage = error.response?.data?.message || "Cập nhật thất bại. Vui lòng thử lại."
      setError(errorMessage)
      toast.error(errorMessage)
      throw error
    } finally {
      setLoading(false)
    }
  }

  /**
   * Kiểm tra quyền truy cập
   * @param {string|Array} requiredRole - Vai trò yêu cầu
   * @returns {boolean} - Có quyền truy cập hay không
   */
  const hasPermission = (requiredRole) => {
    if (!currentUser) return false

    if (Array.isArray(requiredRole)) {
      return requiredRole.includes(currentUser.role)
    }

    if (requiredRole === "admin") {
      return currentUser.role === "admin"
    }

    if (requiredRole === "employee") {
      return currentUser.role === "admin" || currentUser.role === "employee"
    }

    return true // Khách hàng có thể truy cập các trang công khai
  }

  // Giá trị context
  const value = {
    currentUser,
    loading,
    error,
    login,
    register,
    logout,
    updateProfile,
    hasPermission,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export default AuthContext

