<template>
    <div class="container">
        <v-layout justify-center>
            <v-flex xs12 sm6>
                <v-toolbar color="indigo" dark>
                    <!--<v-toolbar-side-icon></v-toolbar-side-icon>-->
                    <v-toolbar-title>Documents List</v-toolbar-title>
                    <!--<v-spacer></v-spacer>-->
                    <!--<v-btn icon>-->
                    <!--<v-icon>search</v-icon>-->
                    <!--</v-btn>-->
                </v-toolbar>
                <v-card>
                    <v-container fluid grid-list-md>
                        <v-layout row wrap>
                            <v-flex v-for="(document, index) in documentsList" :key="document.id">
                                <v-card>
                                    <v-img :class="[ 'document-preview', proccessedDocuments.includes(document.id) ? 'blured' : '']"
                                           :src="document.preview"
                                           height="200px"
                                    >
                                        <v-container
                                                fill-height
                                                fluid
                                                pa-2
                                        >
                                            <v-layout fill-height>
                                                <v-flex xs12 align-end flexbox>
                                                    <span class="headline white--text" v-text="document.id"></span>
                                                </v-flex>
                                            </v-layout>
                                        </v-container>
                                    </v-img>
                                    <v-card-actions>
                                        <v-btn flat
                                               @click="approveDocument(document)"
                                               :disabled="proccessedDocuments.includes(document.id)">Approve
                                            <v-icon>check</v-icon>
                                        </v-btn>
                                        <v-btn flat color="purple"
                                               :disabled="proccessedDocuments.includes(document.id)"
                                               @click="deleteDocument(document)">Delete
                                            <v-icon>delete</v-icon>
                                        </v-btn>
                                        <v-spacer></v-spacer>
                                    </v-card-actions>
                                </v-card>
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-card>
            </v-flex>
        </v-layout>
        <v-bottom-nav
                app
                shift
                :active.sync="bottomNav"
                :value="true"
                color="white"
        >
            <v-btn color="teal" flat value="recent" @click="getDocumentsList()">
                <span>Get another 20</span>
                <v-icon>refresh</v-icon>
            </v-btn>
            <!--<v-btn color="teal" flat value="favorites">-->
            <!--<span>Favorites</span>-->
            <!--<v-icon>favorite</v-icon>-->
            <!--</v-btn>-->

            <!--<v-btn color="teal" flat value="nearby">-->
            <!--<span>Nearby</span>-->
            <!--<v-icon>place</v-icon>-->
            <!--</v-btn>-->
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
                bottomNav: 'recent'

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
                approveDeleteService.getDocuments()
                    .then(resp => {
                            this.documentsList = resp;
                            this.arrayOfIds = this.documentsList.map(item => {
                                return item.id
                            });
                            console.log('docs!', resp)
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })

            },
            deleteDocument(document) {
                approveDeleteService.deleteDocument(document.id)
                    .then(resp => {
                            this.$toaster.error(`Document ${document.id} successfully deleted`);
                            console.log('docs!', resp)
                            this.markAsProccessed(document)
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            approveDocument(document) {

                let arrIds = [];
                arrIds = this.arrayOfIds.filter((singleID) => {
                    return !this.proccessedDocuments.includes(singleID);
                });
                approveDeleteService.approveDocument(arrIds)
                    .then(resp => {
                            this.$toaster.error(`Document ${arrIds} approved`);
                            console.log('docs!', resp)
                            this.markAsProccessed(arrIds)
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

    }

</style>