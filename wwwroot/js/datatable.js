function initializeDataTable() {

    $('.datatable').each(function () {

        const table = $(this);
        const columnCount = table.find('thead th').length;

        table.DataTable({
            lengthMenu: [5, 10, 25, 100],
            pageLength: 10,
            language: {
                lengthMenu: "_MENU_"
            },
            responsive: true,
            lengthChange: true,
            autoWidth: false,
            dom: '<"d-flex justify-content-between align-items-center"<"left-section d-flex"Bl>f>rtip',
            buttons: [
                {
                    extend: 'copy',
                    text: '<i class="fas fa-copy"></i>',
                    titleAttr: 'Copy',
                    exportOptions: {
                        columns: ':not(:last-child)'
                    }
                },
                {
                    extend: 'csv',
                    text: '<i class="fas fa-file-csv"></i>',
                    titleAttr: 'Export as CSV',
                    exportOptions: {
                        columns: ':not(:last-child)'
                    }
                },
                {
                    extend: 'pdf',
                    text: '<i class="fas fa-file-pdf"></i>',
                    titleAttr: 'Export as PDF',
                    exportOptions: {
                        columns: ':not(:last-child)'
                    }
                },
                {
                    extend: 'print',
                    text: '<i class="fas fa-print"></i>',
                    titleAttr: 'Print',
                    exportOptions: {
                        columns: ':not(:last-child)'
                    }
                }
            ],
            columnDefs: [
                {
                    orderable: false,
                    targets: columnCount - 1
                }
            ]
        });

        const wrapper = table.closest('.dataTables_wrapper');

        wrapper.find('.dt-buttons .btn').removeClass('btn-secondary');

        wrapper.find('.dataTables_filter input')
            .attr('placeholder', '🔎 Search');

        wrapper.find('.dataTables_filter label')
            .contents()
            .filter(function () {
                return this.nodeType === 3;
            })
            .remove();
    });
}

$(document).ready(function () {
    console.log("hassan");
    initializeDataTable();
});