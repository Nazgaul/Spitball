<template>
<div>
    <v-toolbar color="indigo" dark>
      <v-toolbar-title>Documents List</v-toolbar-title>
    </v-toolbar>
    <v-container>
      <v-layout wrap row>
        <v-flex xs6 v-for="document in documentsList" :key="document.id">
          <v-card class="elevation-2 ma-2">
            <v-img :src="document.preview ? document.preview: `${require('../../../../assets/img/document.png')}`" aspect-ratio="2.75" contain></v-img>
            <v-card-text>
              <div>
                <b>Id:</b>
                {{document.id}}
              </div>
            </v-card-text>

            <v-card-actions>
              <v-btn
                slot="activator"
                flat
                @click="approveSingleDocument(document)"
                :disabled="proccessedDocuments.includes(document.id)"
              >
                Approve
                <v-icon right>check</v-icon>
              </v-btn>
              <v-btn
                slot="activator"
                flat
                color="purple"
                :disabled="proccessedDocuments.includes(document.id)"
                @click="deleteDocument(document)"
              >
                Delete
                <v-icon right>delete</v-icon>
              </v-btn>
              <v-btn
                slot="activator"
                flat
                color="indigo"
                :disabled="proccessedDocuments.includes(document.id)"
                @click="downloadDocument(document.siteLink)"
              >
                Download
                <v-icon right>cloud_download</v-icon>
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
    <v-bottom-nav app shift :active.sync="bottomNav" :value="true" color="#3f51b5">
      <v-btn class="bottom-nav-btn" dark value="approve" @click="approveDocuments()">
        <div class="btn-text">Approve All</div>
        <v-icon>check</v-icon>
      </v-btn>
    </v-bottom-nav>
  </div>
</template>
<script>
    import approveDeleteService from './approveDeleteService'


    export default {
        data() {
            return {
                infoSuccess: '',
                infoError: '',
                documentsList: {
                    type: Array,
                    default: []
                },
                arrayOfIds: [],
                proccessedDocuments: [],
                bottomNav: 'refresh',
                loading: false

            }
        },
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
                approveDeleteService.getDocuments()
                    .then(resp => {
                            self.documentsList = resp.documents;

                            self.arrayOfIds = self.documentsList.map(item => {
                                return item.id
                            });
                            console.log('docs!', resp)
                            self.loading = false;
                        },
                        (error) => {
                            self.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })

            },
            deleteDocument(document) {
                let singleIdArr = [];
                approveDeleteService.deleteDocument(document.id)
                    .then(resp => {
                            this.$toaster.success(`Document ${document.id} successfully deleted`);
                            singleIdArr.push(document.id);
                            // receives arr with id :: [12]
                            this.markAsProccessed(singleIdArr)
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            //always one el array
            approveSingleDocument(document) {
                let arrSingleId = [];
                arrSingleId.push(document.id);
                approveDeleteService.approveDocument(arrSingleId)
                    .then(resp => {
                            this.$toaster.success(`Document ${arrSingleId} approved`);
                            console.log('docs!', resp);
                            this.markAsProccessed(arrSingleId)
                        },
                        (error) => {
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
                approveDeleteService.approveDocument(arrIds)
                    .then(resp => {
                            this.$toaster.success(`All Documents ${arrIds} approved`);
                            this.getDocumentsList();
                            console.log('docs!', resp)
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            downloadDocument(link) {
              if(!link) return;
              global.location.href = link
            }
        },
        computed: {},
        created() {
            this.getDocumentsList()
        }
    }
</script>

<style lang="scss" scoped>
    
</style>