<template>
    <div class="menuListStudent">
        <v-list-item
            v-for="(link, index) in menuListStudentFilter"
            :to="link.route ? { name: link.route, params: { id: user.id, name: user.name } } : ''"
            :href="link.url || ''"
            :key="index"
            class="link"
            color="#fff"
            sel="menu_row"
            :target="link.url ? '_blank' : ''"
        >
            <v-list-item-action>
                <v-icon class="userMenu_icons" size="18" color="#69687d">{{link.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <v-list-item-title class="userMenu_titles" v-t="link.title"></v-list-item-title>
            </v-list-item-content>
        </v-list-item>
    </div>
</template>

<script>
import * as routNames from '../../../../routes/routeNames'

export default {
    props: {
        menuListStudent: {
            required: true
        },
        menuListTeacher: {
            required: false
        },
        menuListNotLogged: {
            required: false
        }
    },
    computed: {
        user() {
            return this.$store.getters?.accountUser
        },
        menuListStudentFilter() {
            let isSold = this.$store.getters.getIsSold
            return this.menuListStudent.filter(link => {
                // let isUrlMySale = [routNames.MySales].indexOf(link.route) > -1
                let isUrlMySale = link.route === routNames.MySales
                return (isUrlMySale && isSold) || !isUrlMySale
            })
        }
    }
}
</script>