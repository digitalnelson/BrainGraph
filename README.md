BrainGraph is a fully featured windows 8.1 modern application designed to allow users to run NBSm on multimodal imaging data sets.  It is intended to be easy to use yet allow many millions of permutations to be run on highly complex data sets.  These data can be from a variety of sources including fMRI, DTI, PET, EEG, MEG, etc.  The power of BrainGraph is that it works based on brain regions of interest and will happily accept any sort of adjacency matrices referring to similar brain areas.

A full description of the software and its application to Schizophrenia can be found in a paper by Nelson et al (currently in submission).

## Installation ##
BrainGraph is in the process of being submitted to the windows store.  This will be the preferred method of distribution.  It is compatible with x86, x64, and ARM.

If you are interested in helping us test out our application prior to app store approval, a custom developer build is currently available.  

## Developer Build ##

Installation of the developer build is fairly straight forward and requires the user to download a package and run a powershell script which installs a temporary developer certificate and then side loads the application.

### Installation ###
1. Download application package here.
2. Right click on the script file and select run as administrator.

### Removal ###
1. Unknown

## Input Data ##
1. Atlas File - This is a text file with the atlas being used by the particular study.  We have supplied a copy of the AAL (reference here) atlas in a format BrainGraph understands.  AAL.txt can be found here.
2. User List - This is a tab delimited text file which lays out the users to analyze along with a mapping between their subject id (id specific to a subject) and their procedure id (id specific to an adjacency matrix file).
3. Data Folder - This is the folder containing all of the user-specific adjacency matrices.  These are tab delimited text files containing the full connectivity matrix representing the data modality being studied.  This could be fMRI, DTI, EEG, MEG, etc.
