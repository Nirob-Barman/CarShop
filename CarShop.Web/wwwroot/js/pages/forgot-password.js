(function () {
    var forgotForm = document.getElementById('forgotForm');
    if (!forgotForm) return;

    var emailInput = document.getElementById('emailInput');
    var checkUrl = emailInput ? emailInput.dataset.checkUrl : null;

    forgotForm.addEventListener('submit', function (e) {
        e.preventDefault();
        var email = emailInput ? emailInput.value : '';
        if (!email || (typeof validateEmailFormat === 'function' && !validateEmailFormat(email))) return;
        $.ajax({
            url: checkUrl,
            type: 'POST',
            data: { email: email },
            success: function (exists) {
                if (!exists) {
                    document.getElementById('emailError').style.display = 'block';
                } else {
                    document.getElementById('emailError').style.display = 'none';
                    forgotForm.submit();
                }
            },
            error: function () {
                Swal.fire({ icon: 'error', title: 'Oops...', text: 'Error checking email', toast: true, position: 'top-end', timer: 4000, showConfirmButton: false });
            }
        });
    });
})();
