#include "pch.h"
#include "BindableBase.h"

using namespace BrainGraph::Compute::Common;

using namespace Platform;
using namespace Windows::UI::Xaml::Data;

void BindableBase::OnPropertyChanged(String^ propertyName)
{
    //PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
}

ICustomProperty^ BindableBase::GetCustomProperty(Platform::String^ name) 
{
    return nullptr;
}

ICustomProperty^ BindableBase::GetIndexedProperty(Platform::String^ name, Windows::UI::Xaml::Interop::TypeName type) 
{
    return nullptr;
}

Platform::String^ BindableBase::GetStringRepresentation() 
{
    return ""; //this->ToString(); 
}