function confirmCancelBooking(id) {
    Swal.fire({
        title: 'Cancel booking?', text: 'This will mark the booking as Cancelled.', icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, cancel it', cancelButtonText: 'No, keep it'
    }).then(function (result) {
        if (result.isConfirmed) {
            var form = document.getElementById('td-form-' + id);
            var btn = document.createElement('input');
            btn.type = 'hidden'; btn.name = 'status'; btn.value = 'Cancelled';
            form.appendChild(btn);
            form.submit();
        }
    });
}
