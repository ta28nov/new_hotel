"use client"

import { useState, useEffect } from "react"
import { Link } from "react-router-dom"
import { motion } from "framer-motion"
import { FaUser, FaEnvelope, FaLock, FaPhone, FaGoogle, FaFacebookF } from "react-icons/fa"
import Header from "../../components/Header/Header"
import Footer from "../../components/Footer/Footer"
import "./RegisterPage.css"

const RegisterPage = () => {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
    agreeTerms: false,
  })
  const [errors, setErrors] = useState({})
  const [isSubmitting, setIsSubmitting] = useState(false)

  // Cuộn lên đầu trang khi component được tải
  useEffect(() => {
    window.scrollTo(0, 0)
  }, [])

  // Xử lý thay đổi input
  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }))

    // Xóa lỗi khi người dùng nhập lại
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors[name]
        return newErrors
      })
    }
  }

  // Xác thực form
  const validateForm = () => {
    const newErrors = {}

    if (!formData.firstName.trim()) {
      newErrors.firstName = "First name is required"
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = "Last name is required"
    }

    if (!formData.email.trim()) {
      newErrors.email = "Email is required"
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Email is invalid"
    }

    if (!formData.password) {
      newErrors.password = "Password is required"
    } else if (formData.password.length < 8) {
      newErrors.password = "Password must be at least 8 characters"
    }

    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match"
    }

    if (!formData.agreeTerms) {
      newErrors.agreeTerms = "You must agree to the terms and conditions"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  // Xử lý đăng ký
  const handleRegister = (e) => {
    e.preventDefault()

    if (validateForm()) {
      setIsSubmitting(true)

      // Mô phỏng API call
      setTimeout(() => {
        setIsSubmitting(false)
        // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
        window.location.href = "/login"
      }, 1500)
    }
  }

  return (
    <div className="register-page">
      <Header />

      <div className="register-container">
        <div className="container">
          <motion.div
            className="register-form-container"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
          >
            <div className="register-form-content">
              <h1>Create an Account</h1>
              <p>Join our community to access exclusive benefits and manage your reservations.</p>

              <form onSubmit={handleRegister} className="register-form">
                <div className="form-row">
                  <div className="form-group">
                    <label htmlFor="firstName">
                      <FaUser /> First Name
                    </label>
                    <input
                      type="text"
                      id="firstName"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleInputChange}
                      placeholder="Enter your first name"
                      className={errors.firstName ? "error" : ""}
                    />
                    {errors.firstName && <div className="error-message">{errors.firstName}</div>}
                  </div>

                  <div className="form-group">
                    <label htmlFor="lastName">
                      <FaUser /> Last Name
                    </label>
                    <input
                      type="text"
                      id="lastName"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleInputChange}
                      placeholder="Enter your last name"
                      className={errors.lastName ? "error" : ""}
                    />
                    {errors.lastName && <div className="error-message">{errors.lastName}</div>}
                  </div>
                </div>

                <div className="form-row">
                  <div className="form-group">
                    <label htmlFor="email">
                      <FaEnvelope /> Email Address
                    </label>
                    <input
                      type="email"
                      id="email"
                      name="email"
                      value={formData.email}
                      onChange={handleInputChange}
                      placeholder="Enter your email"
                      className={errors.email ? "error" : ""}
                    />
                    {errors.email && <div className="error-message">{errors.email}</div>}
                  </div>

                  <div className="form-group">
                    <label htmlFor="phone">
                      <FaPhone /> Phone Number (Optional)
                    </label>
                    <input
                      type="tel"
                      id="phone"
                      name="phone"
                      value={formData.phone}
                      onChange={handleInputChange}
                      placeholder="Enter your phone number"
                    />
                  </div>
                </div>

                <div className="form-row">
                  <div className="form-group">
                    <label htmlFor="password">
                      <FaLock /> Password
                    </label>
                    <input
                      type="password"
                      id="password"
                      name="password"
                      value={formData.password}
                      onChange={handleInputChange}
                      placeholder="Enter your password"
                      className={errors.password ? "error" : ""}
                    />
                    {errors.password && <div className="error-message">{errors.password}</div>}
                  </div>

                  <div className="form-group">
                    <label htmlFor="confirmPassword">
                      <FaLock /> Confirm Password
                    </label>
                    <input
                      type="password"
                      id="confirmPassword"
                      name="confirmPassword"
                      value={formData.confirmPassword}
                      onChange={handleInputChange}
                      placeholder="Confirm your password"
                      className={errors.confirmPassword ? "error" : ""}
                    />
                    {errors.confirmPassword && <div className="error-message">{errors.confirmPassword}</div>}
                  </div>
                </div>

                <div className="form-group terms">
                  <div className="checkbox-container">
                    <input
                      type="checkbox"
                      id="agreeTerms"
                      name="agreeTerms"
                      checked={formData.agreeTerms}
                      onChange={handleInputChange}
                      className={errors.agreeTerms ? "error" : ""}
                    />
                    <label htmlFor="agreeTerms">
                      I agree to the <Link to="/terms">Terms and Conditions</Link> and{" "}
                      <Link to="/privacy">Privacy Policy</Link>
                    </label>
                  </div>
                  {errors.agreeTerms && <div className="error-message">{errors.agreeTerms}</div>}
                </div>

                <button type="submit" className="btn btn-primary btn-block" disabled={isSubmitting}>
                  {isSubmitting ? "Creating Account..." : "Create Account"}
                </button>
              </form>

              <div className="social-register">
                <p>Or register with</p>
                <div className="social-buttons">
                  <button className="social-button google">
                    <FaGoogle /> Google
                  </button>
                  <button className="social-button facebook">
                    <FaFacebookF /> Facebook
                  </button>
                </div>
              </div>

              <div className="login-link">
                <p>
                  Already have an account? <Link to="/login">Login</Link>
                </p>
              </div>
            </div>

            <div className="register-image">
              <img
                src="https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1470&q=80"
                alt="Luxury Hotel"
              />
              <div className="image-overlay">
                <h2>Join Our Community</h2>
                <p>Create an account to access exclusive member benefits and manage your reservations with ease.</p>
              </div>
            </div>
          </motion.div>
        </div>
      </div>

      <Footer />
    </div>
  )
}

export default RegisterPage

