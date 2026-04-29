using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class EventPhotoEditorItem : ObservableObject
{
    [ObservableProperty]
    private byte[] _imageData = Array.Empty<byte>();
}


