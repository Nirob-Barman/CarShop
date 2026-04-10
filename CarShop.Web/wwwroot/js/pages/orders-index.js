function confirmCancel(btn, orderId) {
    Swal.fire({
        title: 'Cancel Order #' + orderId + '?',
        text: 'This will restore car stock and cannot be undone.',
        icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, cancel it', cancelButtonText: 'Keep order'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
