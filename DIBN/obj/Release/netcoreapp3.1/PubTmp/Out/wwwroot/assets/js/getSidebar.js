
$(function (e) {
    $.ajax({
        url: "@Url.Action('GetRolePermissionModule', 'Permission')",
        method: 'GET',
        async: false,
        cache: false,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (RoleResponse) {
            $.ajax({
                url: "@Url.Action('GetCompanyPermissionModule', 'Permission')",
                method: 'GET',
                async: false,
                cache: false,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (CompanyResponse) {

                    $.ajax({
                        url: "@Url.Action('GetUserPermissionModule', 'Permission')",
                        method: 'GET',
                        async: false,
                        cache: false,
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        success: function (UserResponse) {

                            if (UserResponse.length > 0) {

                                for (let i = 0; i < UserResponse.length; i++) {
                                    $('#' + UserResponse[i]).removeAttr('hidden');
                                    if (UserResponse[i] == 'RoleManagement' || UserResponse[i] == 'RolePermission' || UserResponse[i] == 'CompanyPermission' || UserResponse[i] == 'UserPermission') {
                                        $('#ACM').removeAttr('hidden');
                                    }
                                    if (UserResponse[i] == 'EmployeeServices' || UserResponse[i] == 'CompanyServices' || UserResponse[i] == 'MyRequests') {
                                        $('#Services').removeAttr('hidden');
                                    }
                                    if (UserResponse[i] == 'Invoice' || UserResponse[i] == 'MyAccount') {
                                        $('#MyAccountDetails').removeAttr('hidden');
                                    }
                                }
                            } else if (CompanyResponse.length > 0) {

                                for (let i = 0; i < CompanyResponse.length; i++) {
                                    $('#' + CompanyResponse[i]).removeAttr('hidden');
                                    if (CompanyResponse[i] == 'RoleManagement' || CompanyResponse[i] == 'RolePermission' || CompanyResponse[i] == 'CompanyPermission' || CompanyResponse[i] == 'UserPermission') {
                                        $('#ACM').removeAttr('hidden');
                                    }
                                    if (CompanyResponse[i] == 'EmployeeServices' || CompanyResponse[i] == 'CompanyServices' || CompanyResponse[i] == 'MyRequests') {
                                        $('#Services').removeAttr('hidden');
                                    }
                                    if (CompanyResponse[i] == 'Invoice' || CompanyResponse[i] == 'MyAccount' || CompanyResponse[i] == 'PaymentReceipt') {
                                        $('#MyAccountDetails').removeAttr('hidden');
                                    }
                                }
                            }
                            else {
                                for (let i = 0; i < RoleResponse.length; i++) {

                                    $('#' + RoleResponse[i]).removeAttr('hidden');
                                    if (RoleResponse[i] == 'RoleManagement' || RoleResponse[i] == 'RolePermission' || RoleResponse[i] == 'CompanyPermission' || RoleResponse[i] == 'UserPermission') {
                                        $('#ACM').removeAttr('hidden');
                                    }
                                    if (RoleResponse[i] == 'EmployeeServices' || RoleResponse[i] == 'CompanyServices' || RoleResponse[i] == 'MyRequests') {
                                        $('#Services').removeAttr('hidden');
                                    }
                                    if (RoleResponse[i] == 'Invoice' || RoleResponse[i] == 'MyAccount') {
                                        $('#MyAccountDetails').removeAttr('hidden');
                                    }
                                }
                            }
                        }
                    })
                }
            })
        }
    });
    e.preventDefault();
    return false;
});
