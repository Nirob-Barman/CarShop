(function () {
    var families = Array.isArray(window._gatewayFamilies) ? window._gatewayFamilies : [];
    var familySelect = document.getElementById('familySelect');
    var variantRow = document.getElementById('variantRow');
    var variantSelect = document.getElementById('variantSelect');
    var slugHidden = document.getElementById('slugHidden');
    var gatewayFamilyHidden = document.getElementById('gatewayFamilyHidden');
    var nameInput = document.getElementById('nameInput');
    var autoFilledName = '';

    if (!familySelect || !variantRow || !variantSelect || !slugHidden || !gatewayFamilyHidden || !nameInput) {
        return;
    }

    function showFields(slug) {
        document.querySelectorAll('.gateway-fields').forEach(function (panel) {
            panel.classList.add('d-none');
            panel.querySelectorAll('input').forEach(function (input) {
                input.disabled = true;
            });
        });

        if (!slug) {
            return;
        }

        var activePanel = document.querySelector('.gateway-fields[data-slug="' + slug + '"]');
        if (activePanel) {
            activePanel.classList.remove('d-none');
            activePanel.querySelectorAll('input').forEach(function (input) {
                input.disabled = false;
            });
        }
    }

    function applyGateway(family, variant) {
        gatewayFamilyHidden.value = family ? family.Key : '';
        slugHidden.value = variant ? variant.Slug : '';
        showFields(variant ? variant.Slug : '');

        if (variant && (nameInput.value.trim() === '' || nameInput.value.trim() === autoFilledName)) {
            autoFilledName = variant.DisplayName;
            nameInput.value = variant.DisplayName;
        }
    }

    familySelect.addEventListener('change', function () {
        var selectedFamily = families.find(function (family) {
            return family.Key === familySelect.value;
        });

        variantSelect.innerHTML = '<option value="">-- Select a service type --</option>';
        variantRow.classList.add('d-none');
        applyGateway(null, null);

        if (!selectedFamily) {
            return;
        }

        if (selectedFamily.HasVariants) {
            selectedFamily.Variants.forEach(function (variant) {
                var option = document.createElement('option');
                option.value = variant.Slug;
                option.textContent = variant.VariantLabel;
                variantSelect.appendChild(option);
            });
            variantRow.classList.remove('d-none');
            gatewayFamilyHidden.value = selectedFamily.Key;
            return;
        }

        applyGateway(selectedFamily, selectedFamily.Variants[0]);
    });

    variantSelect.addEventListener('change', function () {
        var selectedFamily = families.find(function (family) {
            return family.Key === familySelect.value;
        });

        if (!selectedFamily) {
            applyGateway(null, null);
            return;
        }

        var selectedVariant = selectedFamily.Variants.find(function (variant) {
            return variant.Slug === variantSelect.value;
        });

        applyGateway(selectedFamily, selectedVariant || null);
    });
})();
