<template>
    <div class="uploadFiles d-flex align-center" v-if="!file.error">
        <div class="fileIndexNumber me-2">{{singleFileIndex + 1}}.</div>
        <div class="uploadFilesWrap d-flex align-center justify-space-between flex-grow-1">
            <div class="d-flex align-center">
                <v-text-field
                    v-model="item.name"
                    :rules="[rules.required, rules.maximumChars]"
                    :label="$t('upload_file_title_label')"
                    class="uploadFileInput"
                    placeholder=" "
                    color="#4c59ff"
                    height="44"
                    hide-details
                    dir="ltr"
                    outlined
                    dense
                >
                </v-text-field>
                <v-icon @click="removeFile" size="12" class="ps-3" color="#b8c0d1">{{$vuetify.icons.values.close}}</v-icon>
            </div>

            <v-switch
                v-model="fileSwitch"
                @change="visibleFile"
                :label="$t('visible')"
            ></v-switch>

        </div>
    </div>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules';

export default {
    name: 'uploadFiles',
    props: {
        singleFileIndex: {},
        item: {}
    },
    data() {
        return {
            fileSwitch: true,
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: value => validationRules.maximumChars(value, 150)
            }
        }
    },
    watch: {
        file(item) {
            this.fileSwitch = item.visible
        }
    },
    computed: {
        file() {
            return this.item
        }
    },
    methods: {
        removeFile() {
            this.$store.commit('deleteFileByIndex', this.singleFileIndex)
        },
        visibleFile(val) {
            this.$store.commit('setVisibleFile', {
                val,
                item: this.item
            });
        }
    },
    created() {
        this.fileSwitch = this.file.visible
    }
}
</script>

<style lang="less">
@import '../../../../styles/colors.less';
.uploadFiles {
    .uploadFileInput {
        min-width: 440px;
    }
    .uploadFilesWrap {
        padding: 0 6px;
        margin: 2px;
        &:hover {
            border: 2px dashed #d8d8df !important;
            margin: 0;
        }
    }
    .fileIndexNumber {
        font-weight: 600;
        color: @global-purple;
    }
}
</style>