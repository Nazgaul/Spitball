<template>
    <cover :fixedHeight="true" @setLoading="loaded">
        <template v-if="loading">
            <div class="coverupload" v-if="$store.getters.getIsMyProfile">
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
    data() {
        return {
            loading: false,
        }
    },
    methods: {
        openEdit() {
            this.$store.commit('addComponent', TUTOR_EDIT_PROFILE)
        },
        loaded() {
            this.loading = true
            this.$emit('setLoading')
        },
    }
}
</script>

<style lang="less">
@import "../../../../styles/mixin";

.coverupload {
  position: absolute;
  padding: 6px;
  z-index: 2;
  color: #fff;
  border-radius: 6px;
  @media (max-width: @screen-xs) {
    position: absolute; // temporary for mobile version till new design
  }
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