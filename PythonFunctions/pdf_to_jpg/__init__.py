#from pdf2image import convert_from_path
import logging
import datetime
import azure.functions as func
import os
import io
import re
from pdf2image import convert_from_bytes
from PIL import Image



def main(req: func.HttpRequest) -> func.HttpResponse:
    from azure.storage.blob import BlobServiceClient
    from azure.storage.blob import ContentSettings
    logging.info('Python HTTP trigger function processed a request.')
    connection_string = os.getenv("AzureWebJobsStorage")
    id = req.params.get('id')

    blob_service_client = BlobServiceClient.from_connection_string(connection_string)

    if id : 
        container_client = blob_service_client.get_container_client("spitball-files")
        file_folder = container_client.list_blobs(f"files/{id}")

        for file in file_folder:
            logging.info(file)
            if(re.search(f"^files/{id}/file-", file.name) and re.search(".pdf$", file.name)):
                
                download_stream  = container_client.download_blob(file)
           
                pages = convert_from_bytes(download_stream.readall(), 200, fmt='jpeg')
                i = 0
                
                for page in pages:
                    cc = blob_service_client.get_container_client(f"spitball-files/files/{id}").get_blob_client(f"out-{i}.jpg")
                   
                    testt = page.tobytes()
                    d = io.BytesIO(testt)
                
                    cc.upload_blob(d)
                    i = i + 1
                    

    if id:
        return func.HttpResponse(
                f"processed {id}",
                status_code=200
        )
    else : 
        return func.HttpResponse(
                f"id is required",
                status_code=200
        )

    # if not name:
    #     try:
    #         req_body = req.get_json()
    #     except ValueError:
    #         pass
    #     else:
    #         name = req_body.get('name')

    # if name:
    #     # MODIFICATION: write the a message to the message queue, using msg.set
    #     msg.set(f"Request made for {name} at {datetime.datetime.now()}")

    #     return func.HttpResponse(f"Hello {name}!")
    # else:
    #     return func.HttpResponse(
    #          "Please pass a name on the query string or in the request body",
    #          status_code=400
    #    )