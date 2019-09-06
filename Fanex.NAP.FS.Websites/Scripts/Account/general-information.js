function general_information() {
    var abnormal = $('#abnormal');
    var dangerous_level_original = $('#dangerous-level-original');
    var dangerous_level_non_live = $('#dangerous-level-non-live');
    var dangerous_level_other_sports = $('#dangerous-level-other-sports');

    function InitGeneralInformation() {
        RenderAbnormal();
        RenderDangerousLevel(dangerous_level_original);
        RenderDangerousLevel(dangerous_level_non_live);
        RenderDangerousLevel(dangerous_level_other_sports);
    }

    function RenderAbnormal() {
        var abnormalValue = $(abnormal).text().trim();
        $(abnormal).css({ 'color': '#000000' });

        switch (abnormalValue) {
            case "1":
                $(abnormal).css('background-color', '#F0B2B2');
                break;
            case "2":
                $(abnormal).css('background-color', '#F2EF4C');
                break;
            case "3":
                $(abnormal).css('background-color', '#F2AA4C');
                break;
            case "4":
                $(abnormal).css('background-color', '#246D17');
                break;
            case "5":
                $(abnormal).css('background-color', '#F22F21');
                break;
            case "6":
                $(abnormal).css('background-color', '#554166');
                break;
            case "7":
                $(abnormal).css('background-color', '#6B371D');
                break;
            case "8":
                $(abnormal).css('background-color', '#797370');
                break;
            case "9":
                $(abnormal).css('background-color', '#4DEB35');
                break;
            case "10":
                $(abnormal).css('background-color', '#35EBDC');
                break;
            default:
                $(abnormal).text('None');
                $(abnormal).css({
                    'background-color': '#FFFFFF',                  
                    'border': '1px solid #2f79ed'
                });
                break;
        }
    }

    function RenderDangerousLevel(dangerousName) {
        var dangerousValue = $(dangerousName).text().trim();

        switch (dangerousValue) {
            case '1':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#000000', 'border': '1px solid #2f79ed' });
                break;
            case '2':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#F3373B', 'border': '1px solid #2f79ed' });
                break;
            case '3':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#E5C05A', 'border': '1px solid #2f79ed' });
                break;
            case '4':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#2B44E6', 'border': '1px solid #2f79ed' });
                break;
            case '5':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#DA8A1B', 'border': '1px solid #2f79ed' });
                break;
            case '6':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#BE1BDA', 'border': '1px solid #2f79ed' });
                break;
            case '7':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#971AAE', 'border': '1px solid #2f79ed' });
                break;
            case '8':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#BE6E12', 'border': '1px solid #2f79ed' });
                break;
            case '9':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#918D88', 'border': '1px solid #2f79ed' });
                break;
            case '10':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#397C12', 'border': '1px solid #2f79ed' });
                break;
            case '11':
                $(dangerousName).css({ 'background-color': '#F3373B' });
                break;
            case '12':
                $(dangerousName).css({ 'background-color': '#F0F337' });
                break;
            case '13':
                $(dangerousName).css({ 'background-color': '#3096E4' });
                break;
            case '14':
                $(dangerousName).css({ 'background-color': '#30D5E4' });
                break;
            case '15':
                $(dangerousName).css({ 'background-color': '#D1B2D6' });
                break;
            case '16':
                $(dangerousName).css({ 'background-color': '#AA623B' });
                break;
            case '17':
                $(dangerousName).css({ 'background-color': '#F3A11D' });
                break;
            case '18':
                $(dangerousName).css({ 'background-color': '#CBC8AA' });
                break;
            case '19':
                $(dangerousName).css({ 'background-color': '#BCB57D' });
                break;
            case '20':
                $(dangerousName).css({ 'background-color': '#E93FDE' });
                break;
            case '21':
                $(dangerousName).css({ 'background-color': '#5A64E5' });
                break;
            default:
                $(dangerousName).text('None');
                $(dangerousName).css(
                    {
                        'background-color': '#FFFFFF',
                        'border': '1px solid #2f79ed'
                    }
                );
                break;
        }
    }   

    InitGeneralInformation();
}

$(general_information);