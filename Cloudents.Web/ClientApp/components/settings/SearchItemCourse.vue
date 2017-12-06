<template>
    <div class="course ma-2" :key="item.id" @click="$_updateMyCourses(item)">
        <div class="code pa-2">{{item.code}}</div>
        <div class="name pa-2">{{item.name}}</div>
    </div>
    </template>
<script>
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
                    this.checked ? courses=courses.filter(i => i.id !== val.id) : courses.push(val);
                    this.updateUser({ myCourses: courses });
                }
            }
    }
    </script>