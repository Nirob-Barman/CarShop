function confirmCancelBooking(btn) {
    Swal.fire({
        title: 'Cancel Booking?',
        text: 'Are you sure you want to cancel this test drive booking?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, cancel it',
        cancelButtonText: 'Keep it'
    }).then(function (result) {
        if (result.isConfirmed) btn.closest('form').submit();
    });
}
