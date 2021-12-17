
function myFunction() {
    var table = document.getElementById("myTable");
    var row = table.insertRow(0);
    var cell1 = row.insertCell(0);
    var cell2 = row.insertCell(1);
    cell1.innerHTML = "NEW CELL1";
    cell2.innerHTML = "NEW CELL2";
}


    function selectServiceProdiverData() {
        document.getElementById('contents').style.visibility = "hidden";
        document.getElementById('load').style.visibility = "visible";
    $.ajax({
        type: "POST",
        url: '/Admin/AuditTrail',
        data: { serviceProvider: $("#ServiceProvider").val() },
        
        success: function (resp, status, xhr) {           
            setTimeout(function () {
                document.getElementById('load').style.visibility = "hidden";
                document.getElementById('contents').style.visibility = "visible";
            }, 2000)
            $("#table_body").html(resp);
        },
        error: function (error) {
           
        }
    })
}

function selectDataInDateRange() {    
    document.getElementById('contents').style.visibility = "hidden";
    document.getElementById('load').style.visibility = "visible";
    $.ajax({
        type: "POST",
        url: '/Admin/DateRangeFilter',
        data: { startDate: $("#StartDate").val(), endDate: $("#EndDate").val(), serviceProvider: $("#ServiceProvider").val(), subFolder: $("#SubFolder").val() },

        success: function (resp, status, xhr) {
            setTimeout(function () {
                document.getElementById('load').style.visibility = "hidden";
                document.getElementById('contents').style.visibility = "visible";
            }, 2000)
            $("#table_body").html(resp);
        },
        error: function (error) {

        }
    })
}

function viewRequest() {

}



