<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <v-icon class="five">sbf-five</v-icon>
            <h3 class="sb-title">You Rock! you finished sooo fast! </h3>
            <h4 class="sb-subtitle mt-2">Please make sure all the details are just right before sale</h4>
        </div>
        <div class="upload-row-2 final-row " style="padding-top: 32px;" @click="changeStep(2)">
            <div class="final-item school" >
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span>Edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-university</v-icon>
                    <span class="item-name">School</span>
                </div>
                <div>
                    <p class="school-name">{{getSchoolName}}</p>
                </div>

            </div>
            <div class="final-item class-selected">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span>Edit</span>
                </div>
                <div class="final-item-wrap">
                    <v-icon class="final-icon">sbf-classes-new</v-icon>
                    <span class="item-name">Class</span>
                </div>
                <span class="class-name" >{{getFileData.courses}}</span>
            </div>
        </div>
        <div class="upload-row-3 final-row">
            <div class="final-item doc-type-selected"  @click="changeStep(3)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span>Edit</span>
                </div>
                <div class="final-item-wrap doc-type-wrap">
                    <v-icon class="final-icon doc-type">sbf-{{getFileData.type}}-note</v-icon>
                    <span class="item-name doc-type-name">{{getFileData.type}} note</span>
                </div>

            </div>
            <div class="final-item tags-selected" @click="changeStep(5)">
                <div class="edit">
                    <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                    <span>Edit</span>
                </div>
                <div class="final-item-wrap mb-1">
                    <v-icon class="final-icon tags">sbf-tag-icon</v-icon>
                    <span class="item-name">Tags</span>

                </div>
                <div class="sb-combo final-tags">
                    <v-chip class="sb-chip-tag" v-for="tag in getFileData.tags">
                                                   <span class="chip-button px-1">
                                                       {{tag}}
                                                   </span>
                    </v-chip>
                </div>
            </div>
        </div>
        <div class="upload-row-4 final-row">
            <div class="legal-wrap">
                <input type="checkbox" class="legal-input" id="legal-ownership" v-model="legalCheck" name="legalyOwn" @change="updateLegal()"/>
                <label for="legal-ownership" class="ml-3 legal-ownership">I legally own this document, and all its legal rights</label>
            </div>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    export default {
        name: "uploadStep_6",
        data() {
            return {
                legalCheck: false
            }
        },
        props: {
            callBackmethods:{
                type: Object,
                default: {},
                required: false
            }
        },
        computed: {
            ...mapGetters({
                getLegal: 'getLegal',
                getFileData: 'getFileData',
                getSchoolName: 'getSchoolName'
            }),
        },
        methods: {
            ...mapActions(['updateLegalAgreement']),

            changeStep(step){
                this.callBackmethods.changeStep(step)
            },
            updateLegal() {
                this.updateLegalAgreement(this.legalCheck);
            },
        },

    }
</script>

<style >

</style>