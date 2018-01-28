import Vue from 'vue'
import Component from './SortAndFilter.vue'

describe('sort and filter component', function () {
    let C;
    beforeEach(()=>{
        C=Vue.extend(Component);
        C.prototype.$router={push({query}){
                console.log(query);
                this.query=query;
                return query;
            }}
    });
    test('snapshot', ()=> {
        let vm=new C();
        let $mounted=vm.$mount();
        expect($mounted.$el.outerHTML).toMatchSnapshot()
    });
    test('update sort', ()=> {
        let cc=new C();
        cc.$route={query:{}};
        cc.updateSort('relevance');
        expect(cc.$router.query.sort).toEqual('relevance');
        cc.updateSort('distance');
        expect(cc.$router.query.sort).toEqual('distance');
    });

    describe('update filter', ()=> {
        let cc;
        beforeEach(()=>{
            cc=new C();
        });
        test('update empty filter', ()=> {
            cc.$route={query:{}};
            cc.updateFilter({id:'source',val:'yifat.com',type:{target:{checked:true}}});
            expect(cc.$router.query.source[0]).toEqual('yifat.com');
            cc.updateFilter({id:'source',val:'yifat.com',type:{target:{checked:false}}});
            expect(cc.$router.query.source.length).toBeFalsy();
        });
        test('remove filters', ()=> {
            cc.setFilteredCourses=jest.fn();
            cc.$route={query:{source:"yifat",course:["1234","11"]}};
            cc.updateFilter({id:'source',val:'yifat',type:{target:{checked:false}}});
            expect(cc.$router.query.source.length).toBeFalsy()
            cc.updateFilter({id:'course',val:'1234',type:{target:{checked:false}}});
            expect(cc.$router.query.course.length).toBe(1);
            expect(cc.setFilteredCourses).toBeCalledWith(["11"]);
        });
        test('update with already exist data', ()=> {
            cc.setFilteredCourses=jest.fn();
            cc.$route={query:{source:'yifat.com',jobType:['a','b'],course:"1234"}};
            cc.updateFilter({id:'source',val:'yifat1.com',type:{target:{checked:true}}});
            expect(cc.$router.query.source.length).toEqual(2);
            cc.$route.query=cc.$router.query;
            cc.updateFilter({id:'jobType',val:'c',type:{target:{checked:true}}});
            expect(cc.$router.query.jobType.length).toEqual(3);
            cc.$route.query=cc.$router.query;
            cc.updateFilter({id:'course',val:'123456',type:{target:{checked:true}}});
            expect(cc.$router.query.course.length).toEqual(2);
            cc.$route.query=cc.$router.query;
            expect(cc.setFilteredCourses).toBeCalledWith(["1234","123456"]);
        });
        test('update course filter and update vuex', ()=> {
            cc.setFilteredCourses=jest.fn();
            cc.$route={query:{}};
            cc.updateFilter({id:'course',val:'1234',type:{target:{checked:true}}});
            expect(cc.$router.query.course[0]).toEqual('1234');
            expect(cc.setFilteredCourses).toBeCalledWith(['1234']);
            cc.updateFilter({id:'course',val:'1234',type:{target:{checked:false}}});
            expect(cc.$router.query.course.length).toBeFalsy();
            expect(cc.setFilteredCourses).toBeCalledWith([]);
            cc.updateFilter({id:'source',val:'yifat.com',type:{target:{checked:false}}});
            !expect(cc.setFilteredCourses).toBeCalled();
        });
        test('filter inPerson update sort', ()=> {
            cc.$route={query:{}};
            cc.updateFilter({id:'filter',val:'inPerson',type:{target:{checked:true}}});
            expect(cc.$router.query.filter[0]).toEqual('inPerson');
            expect(cc.$router.query.sort).toEqual('price');
            cc.$router.query={filter:'inPerson',sort:'distance'};
            cc.$route.query.sort='distance';
            cc.updateFilter({id:'filter',val:'inPerson',type:{target:{checked:false}}});
            expect(cc.$router.query.sort).toEqual('distance');
        });
    });
});