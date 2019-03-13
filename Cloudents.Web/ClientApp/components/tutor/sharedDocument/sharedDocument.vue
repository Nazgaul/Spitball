<template>
    <v-container class="shared-document-container">
        <v-layout row>
            <v-flex>
                <div class="iframe-container">
                    <iframe v-if="sharedDocUrl" :src="sharedDocUrl" frameborder="0"></iframe>
                </div>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    export default {
        name: "sharedDocument",
        data() {
            return {
                docName: 'test document',
            }
        },
        props: {
            propName: {
                type: Number,
                default: 0
            },
        },
        computed: {
            ...mapGetters(['sharedDocUrl', 'roomLinkID', 'isRoomFull']),

        },
        methods: {
            ...mapActions(['updateSharedDocLink']),

        },
        created() {
        console.log('created do shred component')

        }
    }
</script>

<style scoped lang="less">
    .shared-document-container {
        .iframe-container {
            overflow: hidden;
            padding-top: 56.25%;
            position: relative;
        }

        .iframe-container iframe {

            position: absolute;
            top: 0;
            left: 0 /*rtl:ignore*/;
            width: 100%;
            max-width: 1280px;
            height: 100%;
            border: 0;
        }

        /* 4x3 Aspect Ratio */
        .iframe-container-4x3 {
            padding-top: 75%;
        }
    }

</style>