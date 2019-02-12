<template>
    <div class="container">
        <v-layout justify-center>
            <v-flex xs12 sm12>
                <v-toolbar color="indigo" dark>
                    <v-toolbar-title>Documents List</v-toolbar-title>
                </v-toolbar>
                <v-card>
                    <v-container fluid grid-list-md>
                        <v-layout row wrap>
                            <h3 v-if="!documentsList.length && !loading">No documents to display</h3>
                            <h3 v-if="loading">Loading Documents...</h3>
                            <v-flex v-else v-for="(document, index) in documentsList" :key="document.id">
                                <v-card>
                                    <v-img :class="[ 'document-preview', proccessedDocuments.includes(document.id) ? 'blured' : '']"
                                           :src="document.preview"
                                           height="200px">
                                        <v-container fill-height
                                                     fluid
                                                     pa-2>
                                            <v-layout fill-height>
                                            </v-layout>
                                        </v-container>
                                    </v-img>
                                    <v-card-actions>
                                        <v-tooltip left>
                                        <v-btn slot="activator" flat
                                               @click="approveSingleDocument(document)"
                                               :disabled="proccessedDocuments.includes(document.id)">
                                            Approve
                                            <v-icon>check</v-icon>
                                        </v-btn>
                                            <span>Approve Document</span>
                                        </v-tooltip>
                                        <v-tooltip left>
                                        <v-btn slot="activator" flat color="purple"
                                               :disabled="proccessedDocuments.includes(document.id)"
                                               @click="deleteDocument(document)">
                                            Delete
                                            <v-icon>delete</v-icon>
                                            </v-btn>
                                            <span>Delete Document</span>
                                        </v-tooltip>
                                        <v-tooltip left>
                                        <v-btn slot="activator" flat color="red" v-bind:href="document.siteLink" target="_blank">
                                            Download
                                        </v-btn>
                                            <span>Download Document</span>
                                        </v-tooltip>
                                        <v-spacer></v-spacer>
                                    </v-card-actions>
                                </v-card>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card>
            </v-flex>
        </v-layout>
        <v-bottom-nav app
                      shift
                      :active.sync="bottomNav"
                      :value="true"
                      color="#3f51b5">
            <v-btn class="bottom-nav-btn" dark value="approve" @click="approveDocuments()">
                <span class="btn-text">Approve All</span>
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
                            self.documentsList = resp;

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
        },
        computed: {},
        created() {
            this.getDocumentsList()
        }
    }
</script>

<style lang="scss" scoped>
    .container {
        .document-preview {
            &.blured {
                -webkit-filter: grayscale(100%); /* Safari 6.0 - 9.0 */
                filter: grayscale(100%);
            }

        }

        .bottom-nav-btn {
            opacity: 1 !important;
            .btn-text {
                color: #ffffff !important;
                font-size: 16px;
                font-weight: 400;
            }

        }
    }
</style>