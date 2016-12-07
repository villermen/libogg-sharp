A managed .NET library for interaction with libogg.
Allows direct and managed access.

I started this project to losslessly concatenate OGG files.
Turns out OGG concatenation is not possible without silence or re-encoding.
Libvorbis and libvorbisfile have a "lap" function for managing this, but this can only be used after decoding, so I gave up on this project.

Reading and writing of ogg packets using streams is simple using this library.
If that's not what you're here for you can request to add anything libogg-wrapper related.
