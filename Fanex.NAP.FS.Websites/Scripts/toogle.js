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

        $('html, body').css('overflowY', 'hidden'); 
    }

    function CloseNav() {
        toggleBtn.removeClass('toggle-close');
        menu.removeClass('visible');
        coverbg.removeClass('visible');

        $('html, body').css('overflowY', 'auto'); 
    }

    CloseNav();

    toggleBtn.click(HandleNav);
}

$(toogle);