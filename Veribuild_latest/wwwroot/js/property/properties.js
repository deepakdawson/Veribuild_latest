'use strict'
let dt = null;
document.addEventListener('DOMContentLoaded', function () {
    initTable();
    mapUi();
});

function mapUi() {
    document.querySelector('#searchbox').addEventListener('input', function (e) {
        if (dt) {
            dt.search(this.value).draw();
        }
    });
}
function initTable() {
    dt = $('#propertiestable').DataTable({
        "searching": true,
        "ordering": true,
        "lengthChange": false,
        "info": false,
        "autoWidth": false,
        "responsive": {
            breakpoints: [
                { name: 'desktop', width: Infinity },
                { name: 'tablet', width: 1024 },
                { name: 'phone', width: 991 }
            ]
        },
        "columns": [
            { "className": "all" },
            { "className": "min-tablet" },
            { "className": "min-tablet" },
            { "className": "min-tablet" },
            { "className": "min-tablet" },
            { "className": "min-tablet" },
            { "className": "min-tablet" },
        ],
        columnDefs: [
            { orderable: true, className: 'reorder', targets: 3 },
            { orderable: false, targets: '_all' }
        ],
        "dom": 'lrtip',
        "fixedHeader": true,
        "pageLength": 20,
        "language": {
            "oPaginate": {
                "sNext": '<i class="fa-solid fa-chevron-right"></i>',
                "sPrevious": '<i class="fa-solid fa-chevron-left"></i>',
                "sFirst": '<i class="fa fa-chevron-double-left"></i>',
                "sLast": '<i class="fa fa-chevron-double-right"></i>'
            }
        }
    });
}