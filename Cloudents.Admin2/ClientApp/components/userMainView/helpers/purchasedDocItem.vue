<template>
    <div>
        <template>
            <v-layout align-start column v-for="(document, index) in purchasedDocuments" :key="`doc`+index" class="mb-3 px-2 py-2 elevation-2">
                <v-flex xs12 md12 sm12 class="py-1">
                    <span class="label subheading text-xs-left text-md-left mr-3">Document ID:</span>
                    <span class="font-weight-medium">  {{document.id}}</span>

                </v-flex>
                <v-flex class="py-1" xs12 md12 sm12><span class="label subheading text-xs-left text-md-left mr-3">Document name:</span>
                    <span class="font-weight-medium">{{document.title}}</span>

                </v-flex>
                <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">University:</span>
                    <span class="font-weight-medium">{{document.university}}</span>
                </v-flex>
                <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Class:</span>
                    <span class="font-weight-medium">{{document.class}}</span>
                </v-flex>
                <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Document Price:</span>
                    <span class="font-weight-medium">{{document.price}}</span>
                </v-flex>

            </v-layout>
        </template>
    </div>
</template>

<script>
    export default {
        name: "documentItem",
        data() {
            return {}
        },
        props: {
            purchasedDocuments: {},
            filterVal: {
                type: String,
                required: false
            },

        },


        methods: {}

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

