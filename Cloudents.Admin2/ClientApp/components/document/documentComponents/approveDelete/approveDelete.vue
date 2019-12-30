<template>
    <div class="approveDelete">
        <h2 class="mb-4 approveDelete_title">Document List</h2>
  
        <v-data-table
          :headers="headers"
          :items="documentsList"
          class="approveDelete_table"
          disable-initial-sort
          :loading="loading"
          expand
          :rows-per-page-items="[5, 10, 25,{text: 'All', value:-1}]">

            <template slot="items" slot-scope="props">
                <tr @click="props.expanded = !props.expanded">
                    <td class="approveDelete_item">
                      <img class="approveDelete_img" :src="props.item.preview ? props.item.preview: `${require('../../../../assets/img/document.png')}`" />
                    </td>
                    <td class="approveDelete_item">{{props.item.id}}</td>
                    <td class="approveDelete_item">{{props.item.name}}</td>
                    <td class="approveDelete_item">
                        <v-tooltip top>
                            <v-btn
                              slot="activator"
                              icon
                              @click="deleteDocument(props.item)"
                              :disabled="proccessedDocuments.includes(props.item.id)"
                            >
                              <v-icon color="red">close</v-icon>
                            </v-btn>
                            <span>Delete</span>
                        </v-tooltip>
                        <v-tooltip top>
                            <v-btn 
                              slot="activator"
                              icon
                              @click="approveSingleDocument(props.item)"
                              :disabled="proccessedDocuments.includes(props.item.id)"
                            >
                              <v-icon color="green">done</v-icon>
                            </v-btn>
                            <span>Check</span>
                        </v-tooltip>
                        <v-tooltip top>
                          <v-btn
                            slot="activator"
                            flat
                            icon
                            color="indigo"
                            :disabled="proccessedDocuments.includes(props.item.id)"
                            @click="downloadDocument(props.item.siteLink)"
                          >
                            <v-icon>cloud_download</v-icon>
                          </v-btn>
                          <span>Download</span>
                        </v-tooltip>
                    </td>
                </tr>
            </template>
        </v-data-table>

        <div class="text-xs-center approveDelete_sticky mt-4">
          <v-btn
            class="bottom-nav-btn"
            dark
            flat
            value="approve"
            @click="approveDocuments()"
            :disabled="loading">
            <div class="btn-text">Approve All</div>
            <v-icon right>check</v-icon>
          </v-btn>
        </div>
    </div>
</template>

<script>
    import approveDeleteService from './approveDeleteService'

    export default {
      name: 'approveDelete',
      data: () => ({
        documentsList: [],
        arrayOfIds: [],
        proccessedDocuments: [],
        loading: false,
        headers: [
          {text: 'Preview', value: 'preview', sortable: false},
          {text: 'Id', value: 'id', sortable: false},
          {text: 'Name', value: 'name', sortable: false},
          {text: 'Actions', value: 'actions', sortable: false}
        ],
      }),
      methods: {
        markAsProccessed(arrIds) {
          for (let i = 0; i < arrIds.length; i++) {
            this.proccessedDocuments.push(arrIds[i])
          }
          return this.proccessedDocuments
        },
        getDocumentsList() {
          let self = this;
          //clear proccessed array
          self.proccessedDocuments = [];
          self.arrayOfIds = [];
          self.loading = true;
          approveDeleteService.getDocuments().then(resp => {
            self.documentsList = resp.documents;
            self.arrayOfIds = self.documentsList.map(item => {
              return item.id
            });
            console.log('docs!', resp)
            self.loading = false;
          }, (error) => {
            self.$toaster.error('Something went wrong');
            console.log('component accept error', error)
          })
        },
        deleteDocument(document) {
          let singleIdArr = [];
          approveDeleteService.deleteDocument(document.id).then(resp => {
            this.$toaster.success(`Document ${document.id} successfully deleted`);
            singleIdArr.push(document.id);
            // receives arr with id :: [12]
            this.markAsProccessed(singleIdArr)
          }, (error) => {
            this.$toaster.error('Something went wrong');
            console.log('component accept error', error)
          })
        },
        //always one el array
        approveSingleDocument(document) {
          let arrSingleId = [];
          arrSingleId.push(document.id);
          approveDeleteService.approveDocument(arrSingleId).then(resp => {
            this.$toaster.success(`Document ${arrSingleId} approved`);
            console.log('docs!', resp);
            this.markAsProccessed(arrSingleId)
          }, (error) => {
            this.$toaster.error('Something went wrong');
            console.log('component accept error', error)
          })
        },
        approveDocuments() {
          let arrIds = [];
          //filter already deleted or approved, in order to prevent second processing attempt on server
          arrIds = this.arrayOfIds.filter((singleID) => {
            return !this.proccessedDocuments.includes(singleID);
          });
          approveDeleteService.approveDocument(arrIds).then(resp => {
            this.$toaster.success(`All Documents ${arrIds} approved`);
            this.getDocumentsList();
            console.log('docs!', resp)
          }, (error) => {
            this.$toaster.error('Something went wrong');
            console.log('component accept error', error)
          })
        },
        downloadDocument(link) {
          if(!link) return;
          global.location.href = link
        }
      },
      created() {
        this.getDocumentsList()
      }
    }
</script>

<style lang="less">
    .approveDelete {
      .approveDelete_title {
        background: #3f51b5;
        padding: 10px;
        color: white
      }
      .approveDelete_table {
        tr {
          th {
            width: 20% !important; //vuetify
          }
        .approveDelete_item {
          &:first-child {
            padding: 10px !important; //vuetify
          }
          .approveDelete_img {
            width: 100px
          }
        }
        }
      }
      .approveDelete_sticky {
        position: sticky;
        bottom: 0;
        background: #3f51b5;
        padding: 10px;
      }
    }
</style>