"use client"

/**
 * AdminLayout.jsx
 *
 * Vai trò: Layout chung cho các trang quản trị.
 * Chức năng:
 * - Hiển thị sidebar với menu điều hướng
 * - Quản lý trạng thái đóng/mở sidebar
 * - Hiển thị thông tin người dùng đăng nhập
 * - Xử lý đăng xuất
 */

import { useState } from "react"
import { NavLink, Outlet, useNavigate } from "react-router-dom"
import { useContext } from "react"
import  AuthContext  from "../../context/AuthContext"
import {
  FaHome,
  FaBed,
  FaUsers,
  FaCalendarAlt,
  FaCogs,
  FaChartLine,
  FaUserShield,
  FaFileInvoiceDollar,
  FaSignOutAlt,
  FaBars,
  FaTimes,
  FaUser,
} from "react-icons/fa"
import "./AdminLayout.css"

const AdminLayout = () => {
  // State quản lý trạng thái đóng/mở sidebar
  const [sidebarOpen, setSidebarOpen] = useState(true)
  const { currentUser, logout } = useContext(AuthContext)
  const navigate = useNavigate()

  // Xử lý đóng/mở sidebar
  const toggleSidebar = () => {
    setSidebarOpen(!sidebarOpen)
  }

  // Xử lý đăng xuất
  const handleLogout = () => {
    logout()
    navigate("/login")
  }

  // Kiểm tra vai trò admin
  const isAdmin = currentUser?.role === "admin"

  return (
    <div className="admin-layout">
      <div className={`sidebar ${sidebarOpen ? "open" : "closed"}`}>
        <div className="sidebar-header">
          <h2>Quản lý khách sạn</h2>
          <button className="sidebar-toggle" onClick={toggleSidebar}>
            {sidebarOpen ? <FaTimes /> : <FaBars />}
          </button>
        </div>

        <div className="sidebar-user">
          <div className="user-avatar">
            <FaUser />
          </div>
          <div className="user-info">
            <h3>{currentUser?.name || "Người dùng"}</h3>
            <p>{currentUser?.role === "admin" ? "Quản trị viên" : "Nhân viên"}</p>
          </div>
        </div>

        <nav className="sidebar-nav">
          <ul>
            <li>
              <NavLink to="/admin/dashboard" className={({ isActive }) => (isActive ? "active" : "")}>
                <FaHome /> <span>Tổng quan</span>
              </NavLink>
            </li>
            <li>
              <NavLink to="/admin/rooms" className={({ isActive }) => (isActive ? "active" : "")}>
                <FaBed /> <span>Quản lý phòng</span>
              </NavLink>
            </li>
            <li>
              <NavLink to="/admin/customers" className={({ isActive }) => (isActive ? "active" : "")}>
                <FaUsers /> <span>Quản lý khách hàng</span>
              </NavLink>
            </li>
            <li>
              <NavLink to="/admin/bookings" className={({ isActive }) => (isActive ? "active" : "")}>
                <FaCalendarAlt /> <span>Quản lý đặt phòng</span>
              </NavLink>
            </li>
            <li>
              <NavLink to="/admin/services" className={({ isActive }) => (isActive ? "active" : "")}>
                <FaCogs /> <span>Quản lý dịch vụ</span>
              </NavLink>
            </li>
            <li>
              <NavLink to="/admin/invoices" className={({ isActive }) => (isActive ? "active" : "")}>
                <FaFileInvoiceDollar /> <span>Quản lý hóa đơn</span>
              </NavLink>
            </li>

            {isAdmin && (
              <>
                <li>
                  <NavLink to="/admin/reports" className={({ isActive }) => (isActive ? "active" : "")}>
                    <FaChartLine /> <span>Báo cáo thống kê</span>
                  </NavLink>
                </li>
                <li>
                  <NavLink to="/admin/users" className={({ isActive }) => (isActive ? "active" : "")}>
                    <FaUserShield /> <span>Quản lý người dùng</span>
                  </NavLink>
                </li>
              </>
            )}
          </ul>
        </nav>

        <div className="sidebar-footer">
          <button className="logout-button" onClick={handleLogout}>
            <FaSignOutAlt /> <span>Đăng xuất</span>
          </button>
        </div>
      </div>

      <div className={`main-content ${!sidebarOpen ? "sidebar-closed" : ""}`}>
        <Outlet />
      </div>
    </div>
  )
}

export default AdminLayout

