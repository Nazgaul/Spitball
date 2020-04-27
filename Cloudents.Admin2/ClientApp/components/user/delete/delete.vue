<template>
    <v-container class="changeCountry-cont">
        <h2 class="mb-3">Delete User</h2>
        <v-form @submit.prevent="deleteUser">
            <div class="changeCountry">
                    <v-text-field
                        v-model="id"
                        label="Id"
                        single-line
                        solo
                    ></v-text-field>
                <v-flex>
                    <v-btn color="primary" class="white--text" type="submit">Send</v-btn>
                </v-flex>
            </div>
        </v-form>
    </v-container>
</template>

<script>
import deleteUserService from './delete.js';

export default {
    name: 'deleteYser',
    data() {
        return {
         
            id: ''
        }
    },
    methods: {
        deleteUser() {
            deleteUserService.deleteUser(this.id).then(res => {
                this.$toaster.success(`Success: delete user ${this.id}`);
            }, (err) => {
                this.$toaster.error(`Error: delete user ${this.id}`);
            }).finally(() => {
                this.id = '';
            })
        }
    }
}
</script>

<style lang="less">
.changeCountry-cont{
    text-align: center;
    .changeCountry{
        max-width: 400px;
        margin: 0 auto;
    }
}
</style>