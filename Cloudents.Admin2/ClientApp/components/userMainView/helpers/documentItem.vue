<template>
    <div class="document-item-wrap">
        <v-list three-line class="docs-list">
            <template v-for="(document, index) in documents">

                <v-list-tile class="document-tile"
                             :key="'doc'+index"
                             avatar
                             @click=""
                >
                    <v-list-tile-avatar>
                        <img :src="'https://placeimg.com/640/480'"
                             :class=" [ 'document-preview',
                             proccessedDocuments.includes(document.id) ? 'blured' : '',
                             ]" @click="imageView('https://placeimg.com/640/480')">
                    </v-list-tile-avatar>

                    <v-list-tile-content>
                        <v-list-tile-title class="doc-title mb-2"><span class="doc-info-label">Document Title: </span>{{document.name}}
                        </v-list-tile-title>
                        <v-list-tile-sub-title class="doc-subtitle mb-1"><span
                                class="doc-info-label">University: </span>{{document.university}}
                        </v-list-tile-sub-title>
                        <v-list-tile-sub-title class="doc-subtitle mb-1"><span class="doc-info-label">Course: </span>{{document.course}}
                        </v-list-tile-sub-title>
                        <v-list-tile-sub-title class="doc-subtitle mb-1"><span
                                class="doc-info-label">Upload Date: </span>
                            {{document.create | dateFromISO}}
                        </v-list-tile-sub-title>
                        <!--<v-list-tile-sub-title class="doc-subtitle mb-1"><span class="doc-info-label">Reason: </span>{{document.reason}}-->
                        <!--</v-list-tile-sub-title>-->

                    </v-list-tile-content>

                    <v-list-tile-action>
                        <v-tooltip left>
                            <v-btn slot="activator" flat
                                   @click="unflagSingleDocument(document, index)"
                                   :disabled="proccessedDocuments.includes(document.id)">
                                Unflag
                                <v-icon>check</v-icon>
                            </v-btn>
                            <span>UnFlag Document</span>
                        </v-tooltip>
                        <v-tooltip left>
                            <v-btn slot="activator" flat color="purple"
                                   :disabled="proccessedDocuments.includes(document.id)"
                                   @click="deleteDocument(document, index)">
                                Delete
                                <v-icon>delete</v-icon>
                            </v-btn>
                            <span>Delete Document</span>
                        </v-tooltip>

                        <!-- <v-btn flat color="red" v-bind:href="document.siteLink" target="_blank">
                            Link
                        </v-btn> -->
                        <v-spacer></v-spacer>
                    </v-list-tile-action>
                </v-list-tile>
            </template>
        </v-list>
        <v-dialog :max-width="'1280px'" :origin="'bottom center'" :fullscreen="false" v-if="showBigImageDialog"
                  v-model="showBigImageDialog">
            <div class="d-flex" justify-center>
                <v-card class="d-flex" column>
                    <v-icon @click="closeImageView()" class="close-dialog">close</v-icon>

                    <img :src="imageBigSrc" alt="">
                </v-card>
            </div>
        </v-dialog>
    </div>
</template>

<script>
    import flaggedDocumentService from '../../document/documentComponents/flaggedDocument/flaggedDocumentService'

    export default {
        name: "documentItem",
        data() {
            return {
                infoSuccess: '',
                infoError: '',
                arrayOfIds: [],
                proccessedDocuments: [],
                bottomNav: 'refresh',
                imageBigSrc: '',
                showBigImageDialog: false

            }
        },
        props: {
            documents: {},
            updateData: {
                type: Function,
                required: false

            },
        },
        methods: {
            imageView(src) {
                this.imageBigSrc = src;
                this.showBigImageDialog = true;
            },
            closeImageView() {
                this.showBigImageDialog = false;
                this.imageBigSrc = '';
            },
            markAsProccessed(arrIds) {
                for (let i = 0; i < arrIds.length; i++) {
                    this.proccessedDocuments.push(arrIds[i])
                }
                return this.proccessedDocuments
            },
            deleteDocument(document, index) {
                let singleIdArr = [];
                flaggedDocumentService.deleteDocument(document.id)
                    .then(resp => {
                            this.$toaster.success(`Document ${document.id} successfully deleted`);
                            singleIdArr.push(document.id);
                            // receives arr with id :: [12]
                            this.markAsProccessed(singleIdArr);
                            this.updateData(index);
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            //always one el array
            unflagSingleDocument(document, index) {
                let arrSingleId = [];
                arrSingleId.push(document.id);
                flaggedDocumentService.unflagDocument(arrSingleId)
                    .then(resp => {
                            this.$toaster.success(`Document ${arrSingleId} approved`);
                            this.markAsProccessed(arrSingleId);
                            this.updateData(index)
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },
            // unflagDocuments() {
            //     let arrIds = [];
            //     //filter already deleted or approved, in order to prevent second processing attempt on server
            //     arrIds = this.arrayOfIds.filter((singleID) => {
            //         return !this.proccessedDocuments.includes(singleID);
            //     });
            //     flaggedDocumentService.unflagDocument(arrIds)
            //         .then(resp => {
            //                 this.$toaster.success(`All Documents ${arrIds} approved`);
            //                 this.getDocumentsList();
            //                 console.log('docs!', resp)
            //             },
            //             (error) => {
            //                 this.$toaster.error('Something went wrong');
            //                 console.log('component accept error', error)
            //             })
            // },
        }

    }
</script>
<style lang="scss">
    .close-dialog {
        position: absolute;
        top: 0;
        right: 0;
    }

    .document-item-wrap {
        .docs-list {
            .document-tile {
                height: 100%;
                .v-list--two-line {
                    height: 100%!important;
                    &.v-list__tile {
                        height: 100%!important;
                        &:hover{
                            background: #00bcd4;
                        }

                    }
                }
                &:nth-child(even) {
                    background-color: rgba(0, 0, 0, .04);
                }
            }
        }

        .doc-info-label {
            font-size: 14px;
            font-weight: 500;
            color: #000;
        }
    }

</style>


