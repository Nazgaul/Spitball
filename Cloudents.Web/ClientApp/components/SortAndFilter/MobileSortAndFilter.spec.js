import Vue from 'vue'
import Component from './MobileSortAndFilter'
function mount(component, options) {
    const Constructor = Vue.extend(component);
    return new Constructor(options)
}

describe('MobileSortAndFilter.vue', function () {
    let comp;
    beforeEach(() => {
        comp=mount(Component)
    })
    it('should have correct `data`', () => {
        expect(typeof Component.data).toEqual('function');
    })
    it('should init filters', function () {
        comp=mount(Component);
        comp.initFilters({key:'source',value:"yifat.com"});
        expect(comp.filters.source.length).toBeTruthy();
        comp.initFilters();
        expect(comp.filters.source.length).toBeFalsy();
        comp.initFilters([{key:'source',value:"yifat.com"},{key:'source',value:"yifat1.com"},{key:'course',value:"12345"}]);
        expect(comp.filters.source.length).toBe(2);
        expect(comp.filters.course.length).toBe(1);
    });
    describe('apply filters', function () {
        let filterComp;
        beforeEach(() => {
            filterComp=mount(Component);
            filterComp.$router ={push({query}){
                console.log(query);
                this.query=query;
                return query;
            }};
            filterComp.$route={query:{}};
            filterComp.$emit=jest.fn();
            filterComp.setFilteredCourses=jest.fn();
        });
        it('should apply sort',function () {
            filterComp.sort="date";
            filterComp.applyFilters();
            expect(Object.keys(filterComp.$router.query).length).toBe(1);
            expect(filterComp.$router.query.sort).toEqual("date")
        });
        it('should apply one filter', function () {
            filterComp.filters.source=['yifat.com'];
            filterComp.applyFilters();
            expect(Object.keys(filterComp.$router.query).length).toBe(1);
            expect(filterComp.$router.query.source.length).toBeTruthy();
            expect(filterComp.$router.query.source[0]).toBe('yifat.com');
        });
        it('should apply multiple filters', function () {
            filterComp.filters.source=['yifat.com','yifat2.com'];
            filterComp.filters.course=['11','22'];
            filterComp.filters.filter=['inPerson'];
            filterComp.sort="distance";
            filterComp.applyFilters();
            expect(Object.keys(filterComp.$router.query).length).toBe(4);
            expect(filterComp.$router.query.source.length).toBeTruthy();
            expect(filterComp.$router.query.source.length).toBe(2);
            expect(filterComp.$router.query.course.length).toBeTruthy();
            expect(filterComp.$router.query.course.length).toBe(2);
            expect(filterComp.$router.query.filter.length).toBeTruthy();
            expect(filterComp.$router.query.sort).toBe('price');
        });
        it('should apply filter change courses', function () {
            filterComp.filters.course=['11','22'];
            filterComp.filterOptions=[{modelId:'course'}];
            filterComp.filterVal=[{key:'course',value:'11'}];
            //add courses
            filterComp.applyFilters();
            expect(filterComp.setFilteredCourses).toBeCalledWith(['11','22']);
            expect(filterComp.$router.query.course.length).toBeTruthy();
            expect(filterComp.$router.query.course.length).toBe(2);
            filterComp.applyFilters();
            !expect(filterComp.setFilteredCourses).toBeCalled();
            //update not course filters
            filterComp.filters.source=['yifat.com','yifat2.com'];
            filterComp.applyFilters();
            !expect(filterComp.setFilteredCourses).toBeCalled();
            expect(filterComp.$emit).toBeCalledWith('input',false);
            //remove course filters
            filterComp.filters.course=[];
            filterComp.applyFilters();
            expect(filterComp.setFilteredCourses).toBeCalledWith([]);
        });
        test('equal size of courses before and after but different', ()=> {
            filterComp.filterOptions=[{modelId:'course'}];
            filterComp.filters.course=['11','22'];
            filterComp.filterVal=[{key:'course',value:'13'},{key:'course',value:'14'}];
            filterComp.applyFilters();
            expect(filterComp.setFilteredCourses).toBeCalledWith(['11','22']);
        });
    });
    it('should resetFilters', function () {
        comp.$route={query:{q:"yifat"}};
        comp.filters.filter=["inPerson"];
        comp.sortOptions=[{id:"distance",name:"Distance"}];
        comp.sort="price";
        comp.$router ={push({query}){
                console.log(query);
                this.query=query;
                return query;
            }};
        comp.setFilteredCourses=jest.fn();
        comp.applyFilters();
        expect(Object.keys(comp.$router.query).length).toBeGreaterThan(1);
        comp.resetFilters();
        expect(comp.sort).toBe("distance");
        expect(Object.keys(comp.$router.query).length).toBe(1);
        comp.filterOptions=[{modelId:'course'}];
        comp.resetFilters();
        expect(comp.setFilteredCourses).toBeCalledWith([]);


    });
    it('should back action reset filters and sort', function () {
        comp.$emit=jest.fn();
        comp.filterVal=[{key:'source',value:"yifat.com"}];
        comp.sortVal='maor';
        comp.sort='yifat';
        expect(comp.sort).toBe('yifat');
        expect(comp.filters.source.length).toBeFalsy();
        comp.$_backAction();
        expect(comp.filters.source.length).toBeTruthy();
        expect(comp.sort).toBe('maor');
        expect(comp.$emit).toBeCalledWith('input',false);
    });
});