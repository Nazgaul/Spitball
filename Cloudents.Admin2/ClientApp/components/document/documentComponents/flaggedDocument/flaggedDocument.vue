<template>
  <div class="flaggedDocument">
    <h2 class="mb-4 flaggedDocument_title">Document List</h2>

      <v-data-table
        :headers="headers"
        :items="documentsList"
        class="flaggedDocument_table"
        disable-initial-sort
        :loading="loading"
        expand
        :rows-per-page-items="[5, 10, 25,{text: 'All', value:-1}]">

          <template slot="items" slot-scope="props">
              <tr @click="props.expanded = !props.expanded">
                  <td class="flaggedDocument_item">
                    <img class="flaggedDocument_img" @click.prevent="openFlaggedDialogState(props.item.preview)" :src="props.item.preview ? props.item.preview: `${require('../../../../assets/img/document.png')}`" />
                  </td>
                  <td class="flaggedDocument_item">
                    <router-link :to="{name: 'userMainView', query: {id: props.item.id}}" target="_blank">{{props.item.id}}</router-link>
                  </td>
                  <!-- <td class="flaggedDocument_item">{{props.item.flaggedUserEmail}}</td> -->
                  <td class="flaggedDocument_item">{{props.item.reason}}</td>
                  <td class="flaggedDocument_item">
                      <v-tooltip top>
                          <v-btn
                            slot="activator"
                            icon
                            @click="deleteDocument(props.item)"
                            :disabled="proccessedDocuments.includes(props.item.id)"
                          >
                            <v-icon color="red">close</v-icon>
                          </v-btn>
                          <span>Accept flag, delete item</span>
                      </v-tooltip>
                      <v-tooltip top>
                          <v-btn 
                            slot="activator"
                            icon
                            @click="unflagSingleDocument(props.item)"
                            :disabled="proccessedDocuments.includes(props.item.id)"
                          >
                            <v-icon color="green">done</v-icon>
                          </v-btn>
                          <span>Ignore flag, approve item</span>
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

      <v-dialog v-model="flaggedDialogState" depressed max-width="550" content-class="flaggedDialog">
          <img :src="currentImage || require('../../../../assets/img/document.png')" alt="image">
      </v-dialog>

      <!-- <div class="text-xs-center flaggedDocument_sticky mt-4">
        <v-btn
          class="bottom-nav-btn"
          dark
          flat
          value="approve"
          @click="unflagDocuments()"
          :disabled="loading">
          <div class="btn-text">Unflag All</div>
          <v-icon right>check</v-icon>
        </v-btn>
      </div> -->
  </div>
</template>

<script>
import flaggedDocumentService from './flaggedDocumentService.js'

export default {
  name: 'flaggedDocument',
  data: () => ({
    documentsList: [],
    arrayOfIds: [],
    proccessedDocuments: [],
    flaggedDialogState: false,
    loading: true,
    currentImage: null,
    headers: [
      {text: 'Preview', value: 'preview', sortable: false},
      {text: 'Id', value: 'id', sortable: false},
      // {text: 'Email', value: 'email', sortable: false},
      {text: 'Reason', value: 'reason', sortable: true},
      {text: 'Actions', value: 'actions', sortable: false}
    ],
  }),
  methods: {
    markAsProccesed(arrIds) {
      for (let i = 0; i < arrIds.length; i++) {
          this.proccessedDocuments.push(arrIds[i]);
      }
      return this.proccessedDocuments;
    },
    getDocumentsList() {
      let self = this;
      //clear proccessed array
      self.proccessedDocuments = [];
      self.arrayOfIds = [];
      flaggedDocumentService.getDocuments().then(resp => {
        self.documentsList = resp;
        self.arrayOfIds = self.documentsList.map(item => {
            return item.id;
        });
      }, (error) => {
        self.$toaster.error('Something went wrong');
        console.log('component accept error', error);
      }).finally(() => {
        this.loading = false;
      })

    },
    deleteDocument(document) {
        let singleIdArr = [];
        flaggedDocumentService.deleteDocument(document.id)
            .then(() => {
                this.$toaster.success(`Document ${document.id} successfully deleted`);
                singleIdArr.push(document.id);
                // receives arr with id :: [12]
                this.markAsProccesed(singleIdArr);
            },
                (error) => {
                    this.$toaster.error('Something went wrong');
                    console.log('component accept error', error);
                });
    },
    //always one el array
    unflagSingleDocument(document) {
        let arrSingleId = [];
        arrSingleId.push(document.id);
        flaggedDocumentService.unflagDocument(arrSingleId)
            .then(resp => {
                this.$toaster.success(`Document ${arrSingleId} approved`);
                console.log('docs!', resp);
                this.markAsProccesed(arrSingleId);
            },
                (error) => {
                    this.$toaster.error('Something went wrong');
                    console.log('component accept error', error);
                });
    },
    unflagDocuments() {
        let arrIds;
        //filter already deleted or approved, in order to prevent second processing attempt on server
        arrIds = this.arrayOfIds.filter((singleId) => {
            return !this.proccessedDocuments.includes(singleId);
        });
        flaggedDocumentService.unflagDocument(arrIds)
            .then(resp => {
                this.$toaster.success(`All Documents ${arrIds} approved`);
                this.getDocumentsList();
                console.log('docs!', resp);
            },
                (error) => {
                    this.$toaster.error('Something went wrong');
                    console.log('component accept error', error);
                });
    },
    downloadDocument(link) {
      if(!link) return;
      global.location.href = link
    },
    openFlaggedDialogState(image) {
      this.flaggedDialogState = true;
      this.currentImage = image;
    }
  },
  created() {
    this.getDocumentsList();
  }
}

</script>

<style lang="less">
    .flaggedDocument {
      .flaggedDocument_title {
        background: #3f51b5;
        padding: 10px;
        color: white
      }
      .flaggedDocument_table {
        tr {
        .flaggedDocument_item {
          &:first-child {
            padding: 10px !important; //vuetify
          }
          .flaggedDocument_img {
            width: 100px;
          }
        }
        }
      }
    }
    .flaggedDialog {
      background: #fff;
      padding: 10px;
      img {
        vertical-align: middle;
      }
    }
</style>