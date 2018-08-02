import verticals from './services/navigation/vertical-navigation/nav'
//
// export let details = {
//     bookDetails: {
//         filter: [{id: "new", name: "new"}, {id: "rental", name: "rental"}, {id: "eBook", name: "eBook"}, {
//             id: "used",
//             name: "used"
//         }],
//         sort: [{id: "price", name: "price"}]
//     }
// };
// TODO remove in case everything works
// export let actionFunction = {
//     ...verticals,
//     ...details
// };
// export let verticalsList = [];
// export let names = [];
// export let page = [];
// export let verticalsNavbar = [];
// export let verticalsName = [];
//
// for (let i in verticals) {
//     let item = verticals[i].data;
//     verticalsName.push(i);
//     names.push({'id': item.id, 'name': item.name});
//     verticalsNavbar.push(
//         {
//             'id': item.id,
//             'name': item.name,
//             'icon': item.icon
//             //image: item.image
//         });
//     verticalsList.push(verticals[i]);
//     page[i] = {
//         // title: item.resultTitle,
//         //emptyText: item.emptyState,
//         filter: item.filter,
//         sort: item.sort
//     }
// }
// for (let i in details) {
//     let item = details[i];
//     page[i] = {filter: item.filter, sort: item.sort}
// }
//
// verticalsNavbar.push(
//     {
//         'id': 'test',
//         'name': "test",
//         'icon': "test"
//         //image: item.image
//     });