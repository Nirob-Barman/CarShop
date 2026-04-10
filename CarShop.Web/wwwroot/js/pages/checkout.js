document.querySelectorAll('.gateway-card').forEach(function (card) {
    card.addEventListener('click', function () {
        document.querySelectorAll('.gateway-card').forEach(function (c) { c.classList.remove('selected'); });
        card.classList.add('selected');
        document.getElementById('selectedGatewayId').value = card.dataset.id;
        document.getElementById('payBtn').disabled = false;
    });
});
