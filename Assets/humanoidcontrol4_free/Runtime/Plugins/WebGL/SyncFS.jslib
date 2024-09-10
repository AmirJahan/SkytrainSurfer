
var SyncFS = {
    WindowAlert : function(message)
    {
        window.alert(Pointer_stringify(message));
    },
    SyncFiles : function()
    {
        FS.syncfs(false,function (err) {
            console.log('Error: syncfs failed!');
        });
        console.log('Error: syncfs executed');
    }
};

mergeInto(LibraryManager.library, SyncFS);

