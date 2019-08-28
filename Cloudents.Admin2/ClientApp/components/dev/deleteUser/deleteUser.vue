<template>
    <div class="delete-user-container">
        <h1>Delete User</h1>
        <div class="user-inputs-container">
            <v-text-field solo class="user-input-text" placeholder="User Id..." type="text" v-model="userId"/>
        </div>
        <div class="delete-button-container">
            <v-btn :loading="loading" color="red" class="delete-button"  :class="{'disabled': lock}" @click="deleteUser()">Delete</v-btn>
        </div>
    </div>
</template>

<script>
    import { deleteUser } from './deleteUserService.js';

    export default {
        data() {
            return {
                userId: "",
                lock: false,
                loading: false
            }
        },
        methods: {
            deleteUser() {
                this.loading = true;
                if (this.userId === "") return;
                this.lock = true;
                deleteUser(this.userId).then(() => {
                    this.$toaster.success(`User Deleted`);
                    this.userId = "";
                    this.loading = false;

                }, (err) => {
                    console.log(err);
                    this.$toaster.error(`Error: couldn't delete user`)
                }).finally(() => {
                    this.lock = false;
                    this.loading = false;

                })
            }
        }
    }
</script>

<style lang="less" scoped>
    .delete-user-container {
        display: flex;
        flex-direction: column;
        justify-content: space-around;
        height: 100%;
        min-height: 250px;
        .user-inputs-container {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            .user-input-text {
                width: 345px;
            }
        }
        .delete-button-container {
            margin-top: 15px;
            .delete-button {
                &.disabled {
                    background-color: #aeabab;
                    pointer-events: none;
                    color: #837d7d;
                }
            }
        }
    }
</style>
