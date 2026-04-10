function confirmRemoveAlert(btn, carTitle) {
    Swal.fire({
        title: 'Remove Alert?',
        text: 'Stop tracking restock for "' + carTitle + '"?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, remove',
        cancelButtonText: 'Keep alert'
    }).then(function (result) {
        if (result.isConfirmed) btn.closest('form').submit();
    });
}
