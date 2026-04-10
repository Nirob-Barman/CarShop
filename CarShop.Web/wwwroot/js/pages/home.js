$(function () {
    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (!entry.isIntersecting) return;
            var $el = $(entry.target);
            observer.unobserve(entry.target);
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
        observer.observe(this);
    });
});
