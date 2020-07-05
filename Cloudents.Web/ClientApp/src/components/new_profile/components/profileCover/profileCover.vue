<template>
    <cover :fixedHeight="true">
        <template v-if="loading">
            <div class="profileCover coverupload uploadBtn" v-if="$store.getters.getIsMyProfile">
                <v-btn class="white--text" color="rgba(0,0,0,.6)" @click="openEdit" depressed v-ripple="false">
                    <editIcon class="editIcon me-3" width="17" />
                    <span class="editText" v-t="'edit'"></span>
                </v-btn>
            </div>
        </template>
    </cover>
</template>

<script>
import { TUTOR_EDIT_PROFILE } from '../../../pages/global/toasterInjection/componentConsts'
import editIcon from '../../images/edit.svg'
import cover from "../../components/cover.vue";
export default {
    name: 'profileCover',
    components: {
        cover,
        editIcon
    },
    computed: {
        loading() {
            return this.$store.getters.getProfileCoverLoading
        }
    },
    methods: {
        openEdit() {
            this.$store.commit('addComponent', TUTOR_EDIT_PROFILE)
        }
    }
}
</script>

<style lang="less">
.uploadBtn {
    top: 70px;
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