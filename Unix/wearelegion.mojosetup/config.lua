local TOTAL_INSTALL_SIZE = 426791649;
local _ = MojoSetup.translate

Setup.Package
{
    vendor = "pwnee.com",
    id = "wearelegion",
    description = "We Are Legion",
    version = "1.0",
    splash = "splash.bmp",
    superuser = false,
    write_manifest = true,
    support_uninstall = true,
    recommended_destinations =
    {
        MojoSetup.info.homedir,
        "/opt/games",
        "/usr/local/games"
    },

    Setup.Readme
    {
        description = _("We Are Legion README"),
        source = _("Linux.README")
    },

    Setup.Option
    {
        -- !!! FIXME: All this filter nonsense is because
        -- !!! FIXME:   source = "base:///some_dir_in_basepath/"
        -- !!! FIXME: doesn't work, since it wants a file when drilling
        -- !!! FIXME: for the final archive, not a directory. Fixing this
        -- !!! FIXME: properly is a little awkward, though.

        value = true,
        required = true,
        disabled = false,
        bytes = TOTAL_INSTALL_SIZE,
        description = "FEZ",

        Setup.File
        {
            wildcards = "*";
        },

        Setup.DesktopMenuItem
        {
            disabled = false,
            name = "We Are Legion",
            genericname = "WeAreLegion",
            tooltip = _("Gomez is a 2D creature living in a 2D world. Or is he?"),
            builtin_icon = false,
            icon = "We Are Legion.png",
            commandline = "%0/WeAreLegion",
            workingdir = "%0",
            category = "Game"
        }
    }
}

-- end of config.lua ...
