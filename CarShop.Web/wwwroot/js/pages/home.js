$(function () {
    // Lazy-load Tier 2 sections via IntersectionObserver
    var lazyObserver = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (!entry.isIntersecting) return;
            var $el = $(entry.target);
            lazyObserver.unobserve(entry.target);
            $.get($el.data('url'), function (html) {
                if ($.trim(html)) {
                    $el.replaceWith(html);
                } else {
                    $el.remove();
                }
            });
        });
    }, { rootMargin: '300px' });

    $('.lazy-section').each(function () {
        lazyObserver.observe(this);
    });

    // Fade-up scroll animation
    var fadeObserver = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                fadeObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    document.querySelectorAll('.fade-up').forEach(function (el) {
        fadeObserver.observe(el);
    });

    // Navbar scroll state
    var $navbar = $('.cs-navbar');
    function updateNavbar() {
        if (window.scrollY > 10) {
            $navbar.addClass('cs-nav-scrolled');
        } else {
            $navbar.removeClass('cs-nav-scrolled');
        }
    }
    updateNavbar();
    $(window).on('scroll', updateNavbar);
});
