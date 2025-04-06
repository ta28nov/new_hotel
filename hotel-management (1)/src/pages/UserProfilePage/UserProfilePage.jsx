"use client"

import { useState, useEffect } from "react"
import { motion } from "framer-motion"
import { FaUser, FaEnvelope, FaPhone, FaMapMarkerAlt, FaCalendarAlt, FaEdit, FaKey } from "react-icons/fa"
import Header from "../../components/Header/Header"
import Footer from "../../components/Footer/Footer"
import "./UserProfilePage.css"

const UserProfilePage = () => {
  // Sample user data
  const [userData, setUserData] = useState({
    firstName: "John",
    lastName: "Doe",
    email: "john.doe@example.com",
    phone: "+1 (555) 123-4567",
    address: "123 Main Street, New York, NY 10001",
    joinDate: "January 15, 2023",
  })

  const [activeTab, setActiveTab] = useState("profile")
  const [isEditing, setIsEditing] = useState(false)
  const [editData, setEditData] = useState({})
  const [bookings, setBookings] = useState([
    {
      id: "BK123456",
      roomName: "Deluxe Room",
      checkIn: "2023-12-15",
      checkOut: "2023-12-20",
      guests: 2,
      status: "confirmed",
      totalAmount: 1250,
    },
    {
      id: "BK789012",
      roomName: "Executive Suite",
      checkIn: "2024-01-10",
      checkOut: "2024-01-15",
      guests: 3,
      status: "upcoming",
      totalAmount: 1750,
    },
    {
      id: "BK345678",
      roomName: "Standard Room",
      checkIn: "2023-11-05",
      checkOut: "2023-11-08",
      guests: 1,
      status: "completed",
      totalAmount: 450,
    },
  ])

  // Cuộn lên đầu trang khi component được tải
  useEffect(() => {
    window.scrollTo(0, 0)
  }, [])

  // Xử lý chuyển tab
  const handleTabChange = (tab) => {
    setActiveTab(tab)
  }

  // Xử lý bắt đầu chỉnh sửa
  const handleStartEdit = () => {
    setEditData({ ...userData })
    setIsEditing(true)
  }

  // Xử lý thay đổi input
  const handleInputChange = (e) => {
    const { name, value } = e.target
    setEditData((prev) => ({
      ...prev,
      [name]: value,
    }))
  }

  // Xử lý lưu thay đổi
  const handleSaveChanges = () => {
    setUserData({ ...editData })
    setIsEditing(false)
  }

  // Xử lý hủy chỉnh sửa
  const handleCancelEdit = () => {
    setIsEditing(false)
  }

  return (
    <div className="profile-page">
      <Header />

      <div className="profile-container">
        <div className="container">
          <h1 className="profile-title">My Account</h1>

          <div className="profile-content">
            <div className="profile-sidebar">
              <div className="user-info">
                <div className="user-avatar">
                  <img src="https://randomuser.me/api/portraits/men/32.jpg" alt="User Avatar" />
                </div>
                <h3>
                  {userData.firstName} {userData.lastName}
                </h3>
                <p>{userData.email}</p>
              </div>

              <div className="profile-tabs">
                <button
                  className={`tab-button ${activeTab === "profile" ? "active" : ""}`}
                  onClick={() => handleTabChange("profile")}
                >
                  <FaUser /> Profile
                </button>
                <button
                  className={`tab-button ${activeTab === "bookings" ? "active" : ""}`}
                  onClick={() => handleTabChange("bookings")}
                >
                  <FaCalendarAlt /> My Bookings
                </button>
                <button
                  className={`tab-button ${activeTab === "password" ? "active" : ""}`}
                  onClick={() => handleTabChange("password")}
                >
                  <FaKey /> Change Password
                </button>
              </div>
            </div>

            <div className="profile-main">
              {activeTab === "profile" && (
                <motion.div
                  className="profile-section"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5 }}
                >
                  <div className="section-header">
                    <h2>Personal Information</h2>
                    {!isEditing && (
                      <button className="btn-edit" onClick={handleStartEdit}>
                        <FaEdit /> Edit
                      </button>
                    )}
                  </div>

                  {isEditing ? (
                    <div className="edit-form">
                      <div className="form-row">
                        <div className="form-group">
                          <label htmlFor="firstName">First Name</label>
                          <input
                            type="text"
                            id="firstName"
                            name="firstName"
                            value={editData.firstName}
                            onChange={handleInputChange}
                          />
                        </div>
                        <div className="form-group">
                          <label htmlFor="lastName">Last Name</label>
                          <input
                            type="text"
                            id="lastName"
                            name="lastName"
                            value={editData.lastName}
                            onChange={handleInputChange}
                          />
                        </div>
                      </div>
                      <div className="form-row">
                        <div className="form-group">
                          <label htmlFor="email">Email</label>
                          <input
                            type="email"
                            id="email"
                            name="email"
                            value={editData.email}
                            onChange={handleInputChange}
                          />
                        </div>
                        <div className="form-group">
                          <label htmlFor="phone">Phone</label>
                          <input
                            type="tel"
                            id="phone"
                            name="phone"
                            value={editData.phone}
                            onChange={handleInputChange}
                          />
                        </div>
                      </div>
                      <div className="form-group">
                        <label htmlFor="address">Address</label>
                        <input
                          type="text"
                          id="address"
                          name="address"
                          value={editData.address}
                          onChange={handleInputChange}
                        />
                      </div>
                      <div className="form-actions">
                        <button className="btn btn-secondary" onClick={handleCancelEdit}>
                          Cancel
                        </button>
                        <button className="btn btn-primary" onClick={handleSaveChanges}>
                          Save Changes
                        </button>
                      </div>
                    </div>
                  ) : (
                    <div className="profile-details">
                      <div className="detail-item">
                        <div className="detail-icon">
                          <FaUser />
                        </div>
                        <div className="detail-content">
                          <h4>Full Name</h4>
                          <p>
                            {userData.firstName} {userData.lastName}
                          </p>
                        </div>
                      </div>
                      <div className="detail-item">
                        <div className="detail-icon">
                          <FaEnvelope />
                        </div>
                        <div className="detail-content">
                          <h4>Email</h4>
                          <p>{userData.email}</p>
                        </div>
                      </div>
                      <div className="detail-item">
                        <div className="detail-icon">
                          <FaPhone />
                        </div>
                        <div className="detail-content">
                          <h4>Phone</h4>
                          <p>{userData.phone}</p>
                        </div>
                      </div>
                      <div className="detail-item">
                        <div className="detail-icon">
                          <FaMapMarkerAlt />
                        </div>
                        <div className="detail-content">
                          <h4>Address</h4>
                          <p>{userData.address}</p>
                        </div>
                      </div>
                      <div className="detail-item">
                        <div className="detail-icon">
                          <FaCalendarAlt />
                        </div>
                        <div className="detail-content">
                          <h4>Member Since</h4>
                          <p>{userData.joinDate}</p>
                        </div>
                      </div>
                    </div>
                  )}
                </motion.div>
              )}

              {activeTab === "bookings" && (
                <motion.div
                  className="bookings-section"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5 }}
                >
                  <div className="section-header">
                    <h2>My Bookings</h2>
                  </div>

                  {bookings.length > 0 ? (
                    <div className="bookings-list">
                      {bookings.map((booking) => (
                        <div key={booking.id} className={`booking-card ${booking.status}`}>
                          <div className="booking-header">
                            <h3>{booking.roomName}</h3>
                            <span className="booking-status">{booking.status}</span>
                          </div>
                          <div className="booking-details">
                            <div className="booking-info">
                              <p>
                                <strong>Booking ID:</strong> {booking.id}
                              </p>
                              <p>
                                <strong>Check-in:</strong> {booking.checkIn}
                              </p>
                              <p>
                                <strong>Check-out:</strong> {booking.checkOut}
                              </p>
                              <p>
                                <strong>Guests:</strong> {booking.guests}
                              </p>
                              <p>
                                <strong>Total:</strong> ${booking.totalAmount}
                              </p>
                            </div>
                            <div className="booking-actions">
                              <button className="btn btn-secondary">View Details</button>
                              {booking.status === "upcoming" && <button className="btn btn-primary">Modify</button>}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <div className="no-bookings">
                      <p>You don't have any bookings yet.</p>
                      <a href="/rooms" className="btn btn-primary">
                        Browse Rooms
                      </a>
                    </div>
                  )}
                </motion.div>
              )}

              {activeTab === "password" && (
                <motion.div
                  className="password-section"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5 }}
                >
                  <div className="section-header">
                    <h2>Change Password</h2>
                  </div>

                  <form className="password-form">
                    <div className="form-group">
                      <label htmlFor="currentPassword">Current Password</label>
                      <input type="password" id="currentPassword" name="currentPassword" />
                    </div>
                    <div className="form-group">
                      <label htmlFor="newPassword">New Password</label>
                      <input type="password" id="newPassword" name="newPassword" />
                    </div>
                    <div className="form-group">
                      <label htmlFor="confirmPassword">Confirm New Password</label>
                      <input type="password" id="confirmPassword" name="confirmPassword" />
                    </div>
                    <div className="form-actions">
                      <button type="submit" className="btn btn-primary">
                        Update Password
                      </button>
                    </div>
                  </form>
                </motion.div>
              )}
            </div>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  )
}

export default UserProfilePage

