<template>
    <v-list-tile :key="item.id" @click="$_updateMyCourses(item)">
        <v-list-tile-content>
            <v-list-tile-title v-text="item.code"></v-list-tile-title>
            <v-list-tile-sub-title v-text="item.name"></v-list-tile-sub-title>
        </v-list-tile-content>
        <v-list-tile-action>
            <v-list-tile-action-text><input type="checkbox" :value="item.id" :id="item.id" :checked="checked"/></v-list-tile-action-text>
        </v-list-tile-action>
    </v-list-tile>
    </template><script>
        import { mapGetters, mapMutations } from 'vuex'
        export default {
            model: {
                prop: 'value',
                event: 'selected'
            },
            props: { item: { type: Object, required: true }, value: {} },

            computed: {
                ...mapGetters(['myCoursesId','myCourses']),
                checked: function () { return this.myCoursesId.includes(this.item.id)}
            },

            methods: {
                ...mapMutations({updateUser:'UPDATE_USER'}),
                $_updateMyCourses(val) {
                    let courses = this.myCourses
                    this.checked ? courses=courses.filter(i => i.id != val.id) : courses.push(val);
                    this.updateUser({ myCourses: courses });
                }
            }
    }
    </script>