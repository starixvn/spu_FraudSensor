function toogle() {
    var toggleBtn = $('.toggle-btn');
    var menu = $('.main-menu');
    var coverbg = $('.cover-bg');

    function HandleNav() {
        if (toggleBtn.hasClass('toggle-close')) {
            return CloseNav();
        }

        return OpenNav();
    }

    function OpenNav() {
        toggleBtn.addClass('toggle-close');
        menu.addClass('visible');
        coverbg.addClass('visible');
    }

    function CloseNav() {
        toggleBtn.removeClass('toggle-close');
        menu.removeClass('visible');
        coverbg.removeClass('visible');
    }

    CloseNav();

    toggleBtn.click(HandleNav);
}

$(toogle);