// These variables define the URLs where the data for the table comes from
// ControllerURL is the base API endpoint
var ControllerURL = '/api/IncidentListView';
// ListURL is the full URL to fetch the list of incidents
var ListURL = ControllerURL + "/getList";

// Define the columns that the table will show
// Each object represents one column, how to get its data, and how to display it
var columns = [
    {
        "data": null,              // This column doesn’t bind to any data, useful for buttons or checkboxes
        "defaultContent": '',     // Empty content by default
        "width": "7px",           // Make it very narrow
        "visible": true           // It will be visible
    },
    {
        "data": "incidentID",     // Binds to incidentID property from data
        "visible": false,         // This column is hidden but data is available (e.g., for identifying a row)
        "class": "HideCol"        // CSS class for styling hidden columns
    },
    {
        "data": "incidentType",   // Shows the incident type string
        "visible": true,          // This column is shown
        "class": "IncidentType"   // CSS class for styling
    },
    // More columns follow the same pattern: data source, visibility, and optional CSS class or render function
    {
        "data": "incDate",        // Date column
        "visible": true,
        "render": dateRender      // Custom function to format date nicely when displaying
    },
    {
        "data": "activityBefore", // A long text field that might be too wide
        "visible": true,
        render: function (data) { // Custom render to truncate text and keep the table tidy
            return "<div class='text-truncate' style='max-width: 150px'>" + data + "</div>";
        }
    },
    // ...other columns similarly defined, some hidden for internal use
];

// Define buttons above the table for user actions like Add, Edit, Delete
var buttons = [
    { "className": 'btn btn-secondary d-none back', "text": 'Back' },
    { "className": 'btn btn-primary add', "text": 'Report New Incident' },
    { "className": 'btn btn-secondary edit', "text": 'Edit' },
    { "className": 'btn btn-secondary delete', "text": 'Delete' }
];

// Optional settings for the table
var paging = true;       // Enable pagination (break table into pages)
var responsive = false;  // Responsive layout disabled (table won’t auto-adjust columns for small screens)
var colReorder = {       // Allow user to drag columns to reorder them
    enable: true,        // Turn on reorder
    realtime: true,      // Column moves instantly when dragged
    fixedColumnsLeft: 1, // Lock first column so it cannot be moved
};

// Now initialize the DataTable with all above configurations
var table = $('.datatables-main').DataTable({
    paging: paging,               // Pagination on/off
    searching: true,              // Allow search box to filter rows
    responsive: responsive,       // Responsive layout setting
    colReorder: colReorder,       // Column reorder options
    dom:                         // Define the layout of table controls (search box, pagination, buttons, etc.)
        "<'row mb-1  '" +
        "<'col-sm-6 col-md-2 radio centered' >" +
        "<'col-sm-6 col-md-10  ' <'px-2 float-md-right float-left'f> <'px-2 float-md-right'l >> >" +
        "<'row'<'col-sm-12'tr>>" +
        "<'row'<'col-sm-12 col-md-7 'B> <'col-sm-12 col-md-5 float-md-right' p >> " +
        "<'row'<'col-sm-12 col-md-5'i>>",
    columns: columns,             // Use our predefined columns
    stateSave: true,              // Save table state (paging, ordering, search) in browser so it remembers on reload
    select: {                    // Enable row selection
        style: 'single'          // Only one row can be selected at a time
    },
    lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]], // Options for rows per page
    iDisplayLength: 10,          // Default rows per page
    ajax: {                      // Where to load data from
        url: ListURL,            // Our API endpoint that returns JSON data
        dataSrc: ""              // Data comes as a raw array (not nested inside another object)
    },
    processing: true,            // Show a loading indicator while fetching data
    language: {                  // Customize loading message
        loadingRecords: ' ',
        processing: 'Loading...</br><i class="fa fa-spinner fa-spin fa-3x fafw"></i><span class="sr-only">...</span>'
    },
    buttons: { buttons },        // Add our buttons to the table UI
    fixedHeader: {               // Fix header and footer so they stay visible when scrolling
        header: true,
        footer: true,
        footerOffset: 0,
        headerOffset: 0
    },
    initComplete: function () {  // Called after table is fully initialized
        table.buttons().container().appendTo($('.table-buttons')); // Put buttons in a container in the page
        $(".datatables-main").removeClass("d-none");               // Show the table (was hidden initially)
        table.columns('.HideCol').visible(false);                   // Hide columns marked with class 'HideCol'
        UpdateSummary();                                            // Update summary info based on loaded data
    }
});

// Setup event handlers for the Add/Edit/Delete buttons after the table initializes
table.on('init.dt', function () {
    $('.back').on('click', function () {
        console.log("Back Clicked!");
        // You could redirect to a different page here
    });

    $('.add').on('click', function () {
        console.log("Add Clicked!");
        $('#incidentTypeModal').modal('show'); // Show modal to select incident type for adding new record
    });

    $('.edit').attr("disabled", true).on('click', function () {
        var row = table.row({ selected: true }).data();
        if (row) {
            // Redirect user to different edit pages depending on incident type
            console.log("Edit clicked on - " + row.incidentID);
            // ... more logic here based on incidentTypeID ...
        }
    });

    $('.delete').attr("disabled", true).on('click', function () {
        var row = table.row({ selected: true }).data();
        if (row) {
            console.log("Delete clicked on - " + row.incidentID);
            $('#deleteModal').modal('show'); // Show confirmation modal before deleting
        }
    });

    // More UI interactions like handling modal selections, confirmations, and AJAX calls follow here...
});

// Whenever the table is redrawn (e.g., new data loaded, search applied)
table.on('draw', function () {
    UpdateSummary(); // Update any summary or stats you want to show
});

// Function to update summary counts (example)
function UpdateSummary() {
    // Loop through all rows, count incidents by type, update summary UI...
}
