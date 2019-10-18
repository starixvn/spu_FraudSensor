function general_information() {
    var formInformationEvidence = new form_information_evidence();

    function getGeneralInformation() {
        common.showLoading();

        var url = common.modifyRelativeUrl('/Account/GetGeneralInformation');
        var paramsFilter = {};
        paramsFilter.AccountId = ACCOUNTID;
        paramsFilter.CustId = CUSTID;

        $.ajax(url, {
            type: 'POST',
            data: paramsFilter,
            success: function (response) {
                bindGeneralInformation(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
                common.closeLoading();
            }
        });
    }

    function bindGeneralInformation(data) {
        renderUserName(data.GeneralInfo);
        renderAccountStatus(data.GeneralInfo);
        renderLoginInformation(data.GeneralInfo);
        renderAssociation(data.GeneralInfo);
        renderFraudInformation(data.GeneralInfo);

        USERNAME = data.GeneralInfo.Username;        
        SUBSCRIBERID = data.GeneralInfo.SubscriberID;
        SUBSCRIBER = data.GeneralInfo.SubscriberName;

        common.closeLoading();
    }

    function renderUserName(info) {
        var icon = '';
        var userNameInfo = '';

        if (info.Username != '') {
            userNameInfo = info.Username + "(" + info.Username2 + ")";
        } else {
            userNameInfo = info.FirstLoginName;
        }

        if (info.AccountEvidenceStatus == 'Clean') {
            $('#general-info-username-icon').addClass('icon-clean');

            userNameInfo = userNameInfo + ': <label>This account is clean</label>';

        } else if (info.AccountEvidenceStatus == 'CentralPoint') {
            $('#general-info-username-icon').addClass('icon-fraud-flag');
            $('#general-info-username-icon').parent().addClass('red');
            userNameInfo = userNameInfo + ': <label>This account is flagged</label>'
        } else {
            $('#general-info-username-icon').addClass('icon-fraud');
            $('#general-info-username-icon').parent().addClass('red');

            userNameInfo = userNameInfo + ': <label>This account is affected by a link to evidence</label>';
        }
        
        $('#general-info-username').append(userNameInfo);
    }

    function renderAccountStatus(info) {
        $('#general-info-account-status').text(info.AccountStatusToString);

        if (info.AccountStatusName != 'NotYetDeposit') {
            $('#general-info-account-status').addClass('badge-label');

            switch (info.AccountStatusName) {
                case "Open":
                case "CN88_Active":
                    $('#general-info-account-status').css('background-color', '#4F971F');
                    break;
                case "Suspended":
                case "CN88_ViewOnly":
                    $('#general-info-account-status').css('background-color', '#DB2222');
                    break;
                case "CN88_Suspended":
                    $('#general-info-account-status').css('background-color', '#D34315');
                    break;
                case "Closed":
                case "CN88_Inactive":
                case "None":
                    $('#general-info-account-status').css('background-color', '#545454');
                    break;
                default:
                    break;
            }
        }        
    }

    function renderLoginInformation(info) {
        $('#general-info-first-login').text(info.FirstLoginToString);
        $('#general-info-last-login').text(info.LastLoginToString);
        $('#general-info-account-age').text(info.FrequencyLogin + "/" + info.AccountAge + "(times / days)");
    }

    function renderAssociation(info) {
        $('#general-info-associated-accounts').text(info.AssociatedAccounts);
        $('#general-info-associated-devices').text(info.AssociatedDevices);
        $('#general-info-shared-devices').text(info.SharedDevices);
        $('#general-info-noshared-device').text(info.NoSharedDevices);
        $('#general-info-account-exceptions').text(info.AccountExceptions);
    }

    function renderFraudInformation(info) {
        renderEvidences(info.EvidenceTypeCodes);
        $('#general-info-problem-account-status').text(info.ProblemAccount);
        $('#general-info-abnormal').text(info.Abnormal);
        $('#general-info-dangerous-level-original').text(info.DangerousOriginal);
        $('#general-info-dangerous-level-non-live').text(info.DangerousNonLive);
        $('#general-info-dangerous-level-other-sports').text(info.DangerousOtherSports);
        $('#general-info-autosuspend').text(info.AutoSuspend);
        $('#general-info-commited-fraudcase').text(info.CommitedFraudCase);

        renderAbnormal();
        renderDangerousLevel($('#general-info-dangerous-level-original'));
        renderDangerousLevel($('#general-info-dangerous-level-non-live'));
        renderDangerousLevel($('#general-info-dangerous-level-other-sports'));
    }    

    function renderEvidences(evidenceTypeCodes) {
        var html = '';
        var evidences = evidenceTypeCodes.split(';');

        for (var i = 0; i < evidences.length; i++) {
            html = html + '<span class="general-info-evidence-item" style="cursor:pointer;" data-evidence-type-code="' + evidences[i]+'">' + evidences[i] + '</span>' + ';';
        }

        $('#general-info-evidences').append(html.substring(0, html.length - 1));

        $('.general-info-evidence-item').on('click', function () {
            formInformationEvidence.OpenModal($(this).attr('data-evidence-type-code'));
        });
    }

    function renderAbnormal() {
        var abnormalValue = $(abnormal).text().trim();
        var abnormal = $('#general-info-abnormal');

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
                $(abnormal).css({ 'color': '#666666' });
                $(abnormal).removeClass('badge-level');              
                break;
        }
    }

    function renderDangerousLevel(dangerousName) {
        var dangerousValue = $(dangerousName).text().trim();

        switch (dangerousValue) {
            case '1':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#000000' });
                break;
            case '2':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#F3373B' });
                break;
            case '3':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#E5C05A' });
                break;
            case '4':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#2B44E6' });
                break;
            case '5':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#DA8A1B' });
                break;
            case '6':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#BE1BDA' });
                break;
            case '7':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#971AAE' });
                break;
            case '8':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#BE6E12' });
                break;
            case '9':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#918D88' });
                break;
            case '10':
                $(dangerousName).css({ 'background-color': '#FFFFFF', 'color': '#397C12' });
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
                        'background-color': '#FFFFFF'
                    }
                );
                break;
        }
    }   

    getGeneralInformation();
}

$(general_information);