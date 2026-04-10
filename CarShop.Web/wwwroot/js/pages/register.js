(function () {
    // Password strength meter
    var regPassword = document.getElementById('regPassword');
    if (regPassword) {
        regPassword.addEventListener('input', function () {
            var p = this.value;
            if (!p) {
                document.getElementById('passwordStrength').style.display = 'none';
                document.getElementById('passwordRequirements').style.display = 'none';
                return;
            }
            document.getElementById('passwordStrength').style.display = 'block';
            document.getElementById('passwordRequirements').style.display = 'block';

            var checks = [p.length >= 6, /\d/.test(p), /[a-z]/.test(p), /[A-Z]/.test(p), /\W/.test(p)];
            var ids = ['reqLength', 'reqDigit', 'reqLower', 'reqUpper', 'reqSpecial'];
            var score = checks.filter(Boolean).length;
            checks.forEach(function (ok, i) {
                document.getElementById(ids[i]).className = ok ? 'text-success' : 'text-danger';
            });

            var levels = [
                { pct: 20, cls: 'bg-danger',  text: 'Very Weak' },
                { pct: 40, cls: 'bg-warning', text: 'Weak' },
                { pct: 60, cls: 'bg-info',    text: 'Fair' },
                { pct: 80, cls: 'bg-primary', text: 'Strong' },
                { pct: 100, cls: 'bg-success', text: 'Very Strong' }
            ];
            var lvl = levels[score - 1] || levels[0];
            var bar = document.getElementById('strengthBar');
            bar.style.width = lvl.pct + '%';
            bar.className = 'progress-bar ' + lvl.cls;
            document.getElementById('strengthLabel').textContent = lvl.text;
        });
    }

    // Email duplicate check
    var emailInput = document.getElementById('emailInput');
    if (emailInput && emailInput.dataset.checkUrl) {
        emailInput.addEventListener('blur', function () {
            var email = this.value;
            if (!email || (typeof validateEmailFormat === 'function' && !validateEmailFormat(email))) return;
            $.ajax({
                url: emailInput.dataset.checkUrl,
                type: 'POST',
                data: { email: email },
                success: function (exists) {
                    document.getElementById('emailError').style.display = exists ? 'block' : 'none';
                    document.getElementById('submitBtn').disabled = exists;
                }
            });
        });
    }

    // Form submit validation
    var registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            var pwd = document.getElementById('regPassword').value;
            var confirm = document.getElementById('confirmPassword').value;
            var valid = true;

            if (pwd !== confirm) {
                document.getElementById('confirmError').style.display = 'block';
                valid = false;
            } else {
                document.getElementById('confirmError').style.display = 'none';
            }

            var allChecks = [pwd.length >= 6, /\d/.test(pwd), /[a-z]/.test(pwd), /[A-Z]/.test(pwd), /\W/.test(pwd)].every(Boolean);
            if (!allChecks) {
                document.getElementById('passwordRequirements').style.display = 'block';
                valid = false;
            }

            if (!valid) e.preventDefault();
        });
    }
})();
