/**
 * Các endpoint API đặt phòng
 *
 * File này định nghĩa các endpoint liên quan đến quản lý đặt phòng,
 * xử lý trạng thái đặt phòng và thanh toán.
 */
import apiClient from "../apiClient"

const bookingEndpoints = {
  // Các thao tác đặt phòng
  getAllBookings: () => apiClient.get("/bookings"),
  getBookingById: (id) => apiClient.get(`/bookings/${id}`),
  createBooking: (bookingData) => apiClient.post("/bookings", bookingData),
  updateBooking: (id, bookingData) => apiClient.put(`/bookings/${id}`, bookingData),
  deleteBooking: (id) => apiClient.delete(`/bookings/${id}`),

  // Quản lý trạng thái đặt phòng
  updateBookingStatus: (id, status) => apiClient.patch(`/bookings/${id}/status`, { status }),
  checkIn: (id) => apiClient.post(`/bookings/${id}/check-in`),
  checkOut: (id) => apiClient.post(`/bookings/${id}/check-out`),
  cancelBooking: (id, reason) => apiClient.post(`/bookings/${id}/cancel`, { reason }),

  // Lọc đặt phòng
  filterBookings: (filters) => apiClient.get("/bookings/filter", { params: filters }),

  // Đặt phòng của khách hàng
  getCustomerBookings: (customerId) => apiClient.get(`/customers/${customerId}/bookings`),
  getCurrentUserBookings: () => apiClient.get("/bookings/my-bookings"),

  // Liên quan đến thanh toán
  updatePaymentStatus: (id, paymentStatus) =>
    apiClient.patch(`/bookings/${id}/payment`, { payment_status: paymentStatus }),
  processPayment: (bookingId, paymentData) => apiClient.post(`/bookings/${bookingId}/payment`, paymentData),

  // Dịch vụ cho đặt phòng
  addServiceToBooking: (bookingId, serviceData) => apiClient.post(`/bookings/${bookingId}/services`, serviceData),
  removeServiceFromBooking: (bookingId, serviceId) => apiClient.delete(`/bookings/${bookingId}/services/${serviceId}`),
  getBookingServices: (bookingId) => apiClient.get(`/bookings/${bookingId}/services`),

  // Tạo hóa đơn
  createInvoice: (bookingId) => apiClient.post(`/bookings/${bookingId}/invoice`),
  getInvoice: (bookingId) => apiClient.get(`/bookings/${bookingId}/invoice`),
  downloadInvoice: (bookingId) => apiClient.get(`/bookings/${bookingId}/invoice/download`, { responseType: "blob" }),
}

export default bookingEndpoints

