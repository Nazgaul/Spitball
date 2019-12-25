<template>
    <v-card class="uni-card" :class="{'': $vuetify.breakpoint.xsOnly}">
        <!--Main courses view-->
        <router-view ></router-view>
        <!--Dialog with new cousre creation-->
        <sb-dialog :isPersistent="true"
                   :showDialog="getCreateDialogVisibility"
                   :popUpType="'create-university'"
                   :max-width="'706px'"
                   :content-class="'create-university-dialog'">
            <verify-university-creation v-if="!uniCreationVerified"></verify-university-creation>
            <create-university v-else></create-university>
        </sb-dialog>
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
    import verifyUniversityCreation from './createUniversity/verifyUniversityCreation.vue';
    import createUniversity from './createUniversity/createUniversity.vue';
    export default {
        components: {
            sbDialog,
            verifyUniversityCreation,
            createUniversity,

        },
        data() {
            return {
            };
        },
        computed: {
            ...mapGetters(["getCreateDialogVisibility", "uniCreationVerified"]),
        },
        methods: {
            ...mapActions(["updateClasses", "updateUniVerification"]),
        }
    };
</script>


<style lang="less" src="./university.less"></style>