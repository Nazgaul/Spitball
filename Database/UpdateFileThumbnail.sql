  update [File]
  set thumbnailblobaddressuri = 'word_icon.gif'  
  where thumbnailblobaddressuri is null
  and right(blobaddressuri,len(blobaddressuri)-CHARINDEX('.',blobaddressuri)) in ('doc','docx')
  
;
  
  update [File]
  set thumbnailblobaddressuri = 'pdf_icon.gif'  
  where thumbnailblobaddressuri is null
  and right(blobaddressuri,len(blobaddressuri)-CHARINDEX('.',blobaddressuri)) in ('pdf')
;
  
  update [File]
  set thumbnailblobaddressuri = 'ppt_icon.gif'  
  where thumbnailblobaddressuri is null
  and right(blobaddressuri,len(blobaddressuri)-CHARINDEX('.',blobaddressuri)) in ('ppt','pptx')
  
;
  
  update [File]
  set thumbnailblobaddressuri = 'excel_icon.gif'  
  where thumbnailblobaddressuri is null
  and right(blobaddressuri,len(blobaddressuri)-CHARINDEX('.',blobaddressuri)) in ('xls','xlsx');
  
    update [File]
  set thumbnailblobaddressuri = 'music_icon.png'  
  where thumbnailblobaddressuri is null
  and right(blobaddressuri,len(blobaddressuri)-CHARINDEX('.',blobaddressuri)) in (
  '.aac',
'aif',
'.iff',
'.m3u',
'.mid',
'.mp3',
'.mpa',
'.ra', 
'.wav',
'.wma');

 update [File]
  set thumbnailblobaddressuri = 'video_icon.png'  
  where thumbnailblobaddressuri is null
  and right(blobaddressuri,len(blobaddressuri)-CHARINDEX('.',blobaddressuri)) in (
  
'.avi',
'.3g2',
'.3gp',
'.asf',
'.asx',
'.flv',
'.mov',
'.mp4',
'.mpg',
'.rm','.swf',
'.vob',
'.wmv')

;
   update [File]
  set thumbnailblobaddressuri = 'file_icon.png'  
  where thumbnailblobaddressuri is null
  