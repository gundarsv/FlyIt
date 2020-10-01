using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.DataAccess.Repositories;
using FlyIt.Domain.Models;
using FlyIt.Domain.ServiceResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyIt.Domain.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager, IRoleRepository roleRepository, IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.roleRepository = roleRepository;
            this.mapper = mapper;
        }

        public async Task<Result<List<RoleDTO>>> GetRoles()
        {
            try
            {
                var roles = await roleRepository.GetRoles();

                if (roles.Count < 1)
                {
                    return new NotFoundResult<List<RoleDTO>>("Roles not found");
                }

                var result = mapper.Map<List<Role>, List<RoleDTO>>(roles);

                return new SuccessResult<List<RoleDTO>>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<List<RoleDTO>>(ex.Message);
            }
        }

        public async Task<Result<IdentityResult>> AddRole(string userId, string roleId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);

                var role = await roleManager.FindByIdAsync(roleId);

                if (user is null)
                {
                    return new NotFoundResult<IdentityResult>("User not found");
                }

                if (role is null)
                {
                    return new NotFoundResult<IdentityResult>("Role not found");
                }

                var result = await userManager.AddToRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    return new InvalidResult<IdentityResult>(result.Errors.ToString());
                }

                return new SuccessResult<IdentityResult>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<IdentityResult>(ex.Message);
            }
        }

        public async Task<Result<IdentityResult>> CreateRole(string roleName)
        {
            try
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = roleName
                });

                if (!roleResult.Succeeded)
                {
                    return new InvalidResult<IdentityResult>(roleResult.Errors.ToString());
                }

                return new SuccessResult<IdentityResult>(roleResult);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<IdentityResult>(ex.Message);
            }
        }

        public async Task<Result<IdentityResult>> RemoveRole(string userId, string roleId)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);

                var role = await roleManager.FindByIdAsync(roleId);

                if (user is null)
                {
                    return new NotFoundResult<IdentityResult>("User not found");
                }

                if (role is null)
                {
                    return new NotFoundResult<IdentityResult>("Role not found");
                }

                var result = await userManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    return new InvalidResult<IdentityResult>(result.Errors.FirstOrDefault().Description);
                }

                return new SuccessResult<IdentityResult>(result);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<IdentityResult>(ex.Message);
            }
        }

        public async Task<Result<IdentityResult>> DeleteRole(int id)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(id.ToString());

                if (role is null)
                {
                    return new NotFoundResult<IdentityResult>("Role not found");
                }

                var roleResult = await roleManager.DeleteAsync(role);

                if (!roleResult.Succeeded)
                {
                    return new InvalidResult<IdentityResult>(roleResult.Errors.ToString());
                }

                return new SuccessResult<IdentityResult>(roleResult);
            }
            catch (Exception ex)
            {
                return new UnexpectedResult<IdentityResult>(ex.Message);
            }
        }
    }
}
