<template>
    <cover :mainCoverImage="true">
        <template v-if="loading">
            <div class="profileCover profileUploadBtn ma-2" v-if="$store.getters.getIsMyProfile">
                <v-btn class="white--text" color="rgba(0,0,0,.6)" @click="openEdit" depressed v-ripple="false">
                    <editIcon class="editIcon me-3" width="17" />
                    <span class="editText" v-t="'edit'"></span>
                </v-btn>
            </div>
        </template>
    </cover>
</template>

<script>
import editIcon from '../../images/edit.svg'
import cover from "../../components/cover.vue";
export default {
    name: 'profileCover',
    components: {
        cover,
        editIcon
    },
    data() {
        return {
            newCoverImage: undefined,
        }
    },
    computed: {
        drawer() {
            return this.$store.getters.getProfileCoverDrawer
        },
        loading() {
            return this.$store.getters.getProfileCoverLoading
        }
    },
    methods: {
        openEdit() {
            this.$store.commit('setToggleProfileDrawer', !this.drawer)
            this.$store.commit('resetAccount')
            this.$store.commit('resetPreviewCover')
        },
        setPreviewCoverImage(file) {
            this.newCoverImage = window.URL.createObjectURL(file);
        },
    }
}
</script>

<style lang="less">
.profileUploadBtn {
    z-index: 2;
    position: fixed;
    .editIcon {
        path {
            fill: #fff;
        }
    }
    .editText {
        font-size: 16px;
        font-weight: 600;
    }
}
</style>