<template>
    <div class="item-wrap">
        <v-list three-line class="docs-list">
            <template v-for="(document, index) in documents" v-if="isVisible(document.state)">

                <v-list-tile class="document-tile"
                             :key="'doc'+index"
                             avatar
                             @click=""
                >

                    <v-list-tile-avatar v-if="!isDeleted">
                        <img :src="document.preview ?  document.preview : ''"
                             :class=" [ 'document-preview',
                             proccessedDocuments.includes(document.id) ? 'blured' : '',
                             ]" @click="imageView(document.preview)">
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
                    </v-list-tile-content>

                    <v-list-tile-action v-if="!isDeleted">
                        <v-tooltip left attach="tooltip-1" lazy>
                            <v-btn slot="activator" flat class="doc-action tooltip-1 " v-if="!isOk"
                                   @click="isFlagged ? unflagSingleDocument(document, index) : approveSingleDocument(document, index)"
                                   :disabled="proccessedDocuments.includes(document.id)">
                                <v-icon>check</v-icon>
                            </v-btn>
                            <span>{{isFlagged ? 'UnFlag Document' : 'Approve Document' }}</span>
                        </v-tooltip>
                        <v-tooltip left attach="tooltip-2" lazy>
                            <v-btn slot="activator" flat color="purple" class="doc-action tooltip-2"
                                   :disabled="proccessedDocuments.includes(document.id)"
                                   @click="deleteDocument(document, index)">
                                <v-icon>delete</v-icon>
                            </v-btn>
                            <span>Delete Document</span>
                        </v-tooltip>
                        <v-tooltip left attach="tooltip-3" lazy>
                            <v-btn slot="activator" class="doc-action tooltip-3" flat :href="document.siteLink"
                                   target="_blank">

                                <v-icon>link</v-icon>
                            </v-btn>
                            <span>Download</span>
                        </v-tooltip>
                        <v-spacer></v-spacer>
                    </v-list-tile-action>
                </v-list-tile>
            </template>
        </v-list>
        <v-dialog :max-width="'1280px'" :origin="'bottom center'" :fullscreen="false" v-if="showBigImageDialog"
                  v-model="showBigImageDialog">
            <div class="" justify-center>
                <v-card class="justify-center" column>
                    <v-icon @click="closeImageView()" class="close-dialog">close</v-icon>
                    <img :src="imageBigSrc" alt="">
                </v-card>
            </div>
        </v-dialog>
    </div>
</template>

<script>
    import flaggedDocumentService from '../../document/documentComponents/flaggedDocument/flaggedDocumentService';
    import approveDeleteService from '../../document/documentComponents/approveDelete/approveDeleteService';
    import {mapActions} from 'vuex';
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
                showBigImageDialog: false,


            }
        },
        props: {
            documents: {},
            filterVal: {
                type: String,
                required: false
            },

        },
        computed: {
            ...mapActions(['deleteDocumentItem']),
            isOk() {
                return this.filterVal === 'ok'
            },
            isPending() {
                return this.filterVal === 'pending'
            },
            isFlagged() {
                return this.filterVal === 'flagged'
            },
            isDeleted(){
                return this.filterVal === 'deleted'
            }
        },
        methods: {
            isVisible(itemState) {
                return itemState.toLowerCase() === this.filterVal.toLowerCase();
            },
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
                            this.deleteDocumentItem(index);
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
            //always one el array
            approveSingleDocument(document, index) {
                let arrSingleId = [];
                arrSingleId.push(document.id);
                approveDeleteService.approveDocument(arrSingleId)
                    .then(resp => {
                            this.$toaster.success(`Document ${arrSingleId} approved`);
                            console.log('docs!', resp);
                            this.markAsProccessed(arrSingleId);
                            // this.deleteDocumentItem(index);
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                            console.log('component accept error', error)
                        })
            },

        }

    }
</script>
<style lang="scss">
    .close-dialog {
        position: absolute;
        top: 0;
        right: 0;
    }

    .item-wrap {

        .doc-action {
            height: 24px;
        }
        .docs-list {
            .document-tile {
                height: 100%;
                .v-list--two-line {
                    height: 100% !important;
                    &.v-list__tile {
                        height: 100% !important;
                        &:hover {
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

